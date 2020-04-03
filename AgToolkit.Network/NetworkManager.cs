using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.AgToolkit.Network.Interfaces;
using FramaToolkit;
using UnityEngine;
using UnityEngine.Events;

namespace AgToolkit.AgToolkit.Network
{
	[Serializable]
	public struct ConnectionConfig
	{
		[SerializeField]
		private string _ServerAddress;
		[SerializeField]
		private int _ServerPort;

		public string ServerAddress => _ServerAddress;
		public int ServerPort => _ServerPort;

		public ConnectionConfig(string address = null, int port = 0)
		{
			_ServerAddress = address;
			_ServerPort = port;
		}
	}

	public struct RoomData
	{
		public string Name { get; private set; }

		public int NbUsers { get; private set; }

		public bool IsOpen;

		public RoomData(string name, int nbUsers, bool isOpen)
		{
			Name = name;
			NbUsers = nbUsers;
			IsOpen = isOpen;
		}

	}

	public class NetworkManager : Singleton<NetworkManager>
	{
		[SerializeField]
		private GameObject _NetworkImplementation = null;
		[SerializeField]
		private bool _BroadcastToFindServer = true;
		[SerializeField]
		private float _SynchroTimeout = 60f;
		[SerializeField]
		private float _RequestInterval = 5f;

		private NetworkDiscovery _NetworkDiscovery = null;

		public INetworkImplementation Manager { get; private set; } = null;
		public ConnectionConfig NetworkConnectionConfig;
		public bool BroadcastToFindServer => _BroadcastToFindServer;

		public UnityEvent ServerFoundEvent;
		public UnityEvent ConnectedEvent;
		public UnityEvent JoinMainLobbyEvent;
		public UnityEvent RoomCreatedEvent;
		public UnityEvent JoinRoomEvent;
		public UnityEvent UserJoinEvent;
		public UnityEvent PlayersSynchronizedEvent;
		public UnityEvent LeftRoomEvent;
		public UnityEvent UserLeftEvent;
		public UnityEvent DisconnectedEvent;

		protected override void Awake()
		{
			base.Awake();
			Debug.Assert(_NetworkImplementation != null, "Network Implementation reference is missing in NetworkManager.");
			Manager = _NetworkImplementation?.GetComponent<INetworkImplementation>();
			Debug.Assert(Manager != null, "Network Implementation need to have an INetworkImplementation.");
			Manager.Init();

			Dictionary<string, Action<object[]>> actions = new Dictionary<string, Action<object[]>>
			{
				{nameof(RequestStatus), RequestStatus},
				{nameof(PlayersAreSynchronized), PlayersAreSynchronized}
			};
			Manager.SetupRemoteAction(gameObject, actions);
		}

		protected override void OnApplicationQuit()
		{
			base.OnApplicationQuit();
			Manager.Disconnect();
			Clear();
		}

		private void StopBroadcasting()
		{
			if (_BroadcastToFindServer)
			{
				_NetworkDiscovery?.StopThread();
			}
		}

		private void RequestStatus(object[] args)
		{
			int[] usersNotReady = (int[])args[0];

			bool localUserNotReady = usersNotReady.Contains(UserManager.Instance.LocalUser.NetworkId);

			if (localUserNotReady)
			{
				UserManager.Instance.SendReadyStatus();
			}
		}

		private void PlayersAreSynchronized(object[] args)
		{
			SyncPosition(true);
			PlayersSynchronizedEvent.Invoke();
		}

		private void SyncPosition(bool active)
		{
			foreach (User u in UserManager.Instance.Users)
			{
				Manager.SyncUserPosition(u.gameObject, active);
			}
		}

		public void DiscoverServer()
		{
			_NetworkDiscovery = new NetworkDiscovery();
			_NetworkDiscovery.StartNetworkDiscovery();
		}

		public void Clear()
		{
			NetworkConnectionConfig = new ConnectionConfig();
			Manager = null;

			if (_NetworkDiscovery == null)
			{
				return;
			}

			StopBroadcasting();
			_NetworkDiscovery = null;
		}

		public void StartSynchronize()
		{
			foreach (User u in UserManager.Instance.Users)
			{
				u.IsReady = false;
			}
		}

		public IEnumerator WaitOtherUsers()
		{
			float timerTimeOut = 0f;
			float requestInterval = 0f;

			while (!UserManager.Instance.AreReady())
			{
				if (UserManager.Instance.LocalUser.UserRole == EnumUserRole.Master)
				{
					timerTimeOut += Time.deltaTime;
					requestInterval += Time.deltaTime;

					if (timerTimeOut >= _SynchroTimeout)
					{
						foreach (User u in UserManager.Instance.GetUsersNotReady())
						{
							Manager.KickUser(u.NetworkId);
							yield return null;
						}
					}
					else if (requestInterval >= _RequestInterval)
					{
						Manager.ExecuteRemoteAction(gameObject, nameof(RequestStatus), true, UserManager.Instance.GetUsersNotReady().Select(u => u.NetworkId).ToArray());
						requestInterval = 0f;
					}
				}
				yield return null;
			}

			Manager.ExecuteRemoteAction(gameObject, nameof(PlayersAreSynchronized), true);
		}

		#region EventsImplementation

		public void OnServerFound(ConnectionConfig config)
		{
			NetworkConnectionConfig = config;
			ServerFoundEvent.Invoke();
		}

		public void OnConnect()
		{
			StopBroadcasting();
			Debug.Log($"Connected to {NetworkConnectionConfig.ServerAddress}:{NetworkConnectionConfig.ServerPort}.");
			ConnectedEvent.Invoke();
		}

		public void OnMainLobbyJoined()
		{
			UserManager.Instance.LocalUser.SetInfo(EnumUserRole.Spectator, EnumUserType.Network, 0); // Set LocalUser as an network user
			JoinMainLobbyEvent.Invoke();
		}

		public void OnRoomCreated()
		{
			UserManager.Instance.LocalUser.SetInfo(EnumUserRole.Master, EnumUserType.Network, 1);
			Debug.LogWarning("Room has been created");
			RoomCreatedEvent.Invoke();
		}

		public void OnRoomJoined(int networkId, List<int> otherPlayersNetworkId, string roomName)
		{
			if (UserManager.Instance.LocalUser.NetworkId <= 0)
			{
				UserManager.Instance.LocalUser.SetInfo(EnumUserRole.Spectator, EnumUserType.Network, networkId);
			}

			foreach (int otherNetworkId in otherPlayersNetworkId)
			{
				EnumUserRole role = otherNetworkId == 1 ? EnumUserRole.Master : EnumUserRole.Spectator;
				UserManager.Instance.Register(role, EnumUserType.Network, otherNetworkId);
			}

			UserManager.Instance.LocalUser.CurrentRoom = roomName;
			Debug.LogWarning($"room joined with {UserManager.Instance.LocalUser.UserRole} role.");
			JoinRoomEvent.Invoke();
		}

		public void OnUserJoin(int networkId)
		{
			UserManager.Instance.Register(EnumUserRole.Spectator, EnumUserType.Network, networkId); // create the new user
			Debug.LogWarning($"Player joined networkID -> {networkId}");
			UserJoinEvent.Invoke();
		}

		public void OnLeftRoom()
		{
			Debug.LogWarning("OnLeftRoom");
			List<User> users = UserManager.Instance.Users;
			SyncPosition(false);

			foreach (User u in users)
			{
				if (u.LocalId != UserManager.Instance.LocalUser.LocalId)
				{
					Manager.KickUser(u.NetworkId);
					UserManager.Instance.UnRegister(u.NetworkId);
				}
			}

			UserManager.Instance.LocalUser.CurrentRoom = null;
			Manager.JoinMainLobby();
			LeftRoomEvent.Invoke();
		}

		public void OnUserLeft(int userNetworkId)
		{
			Debug.LogWarning("Player left");
			UserManager.Instance.UnRegister(userNetworkId);
			UserLeftEvent.Invoke();
		}

		public void OnDisconnect(string message)
		{
			Debug.Log($"[NETWORK MANAGER] Disconnected from {NetworkConnectionConfig.ServerAddress}:{NetworkConnectionConfig.ServerPort} -> {message}.");
			Manager.ClearRoom();
			DisconnectedEvent.Invoke();
		}
		#endregion

	}
}