using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AgToolkit.Network
{
	public class PhotonNetworkImplementation : MonoBehaviour, INetworkImplementation
	{
		private ConnectionConfig _ServerConnectionConfig;
		private readonly Dictionary<string, RoomData> _CachedRoomInfos = new Dictionary<string, RoomData>();

		/// <summary>SetupPhotonServer is a method in the PhotonNetworkImplementation class.</summary>
		/// <remarks>Setup the settings of the photon server.</remarks>
		private void SetupPhotonServer()
		{
			_ServerConnectionConfig = NetworkManager.Instance.NetworkConnectionConfig;
			Debug.Assert(!string.IsNullOrEmpty(_ServerConnectionConfig.ServerAddress), "Photon Server not found");
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = _ServerConnectionConfig.ServerAddress;
			PhotonNetwork.PhotonServerSettings.AppSettings.Port = _ServerConnectionConfig.ServerPort;
			PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
			PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ConnectionProtocol.Udp;
			PhotonNetwork.ConnectUsingSettings();
		}

		/// <summary>UpdateRoomList is a method in the PhotonNetworkImplementation class.</summary>
		/// <remarks> Cache the room list to keep updated the roomList of the main lobby.</remarks>
		public void UpdateRoomList(List<RoomInfo> roomList)
		{
			foreach (RoomInfo info in roomList)
			{
				// Remove room from cached room list if it got closed, became invisible or was marked as removed
				if (!info.IsVisible || info.RemovedFromList)
				{
					if (_CachedRoomInfos.ContainsKey(info.Name))
					{
						_CachedRoomInfos.Remove(info.Name);
					}

					continue;
				}
				AddRoom(new RoomData(info.Name, info.PlayerCount, info.IsOpen));
			}
			
			NetworkManager.Instance.RoomListUpdatedEvent.Invoke();
		}

		public void AddRoom(RoomData roomData)
		{
			// Update cached room info
			if (_CachedRoomInfos.ContainsKey(roomData.Name))
			{
				_CachedRoomInfos[roomData.Name] = roomData;
			}
			// Add new room info to cache
			else
			{
				_CachedRoomInfos.Add(roomData.Name, roomData);
			}
		}

		#region ImplementationInterface
		public void Init()
		{
			NetworkManager.Instance.gameObject.AddComponent(typeof(PunCallbackImplementation));
			NetworkManager.Instance.gameObject.GetComponent<PunCallbackImplementation>()?.Init(this);
		}

		/// <summary>Connect is a method in the INetworkImplementation interface.</summary>
		/// <remarks>OnConnectedToMaster from PunCallbackImplementation is called on success.</remarks>
		public void Connect()
		{
			if (!NetworkManager.Instance.BroadcastToFindServer)
			{
				Debug.Assert(!string.IsNullOrEmpty(NetworkManager.Instance.NetworkConnectionConfig.ServerAddress), "Photon server address is missing and broadcasting is disable.");
				Debug.Assert(NetworkManager.Instance.NetworkConnectionConfig.ServerPort != 0, "Photon server port is missing and broadcasting is disable.");
				SetupPhotonServer();
			}
			else
			{
				NetworkManager.Instance.ServerFoundEvent.AddListener(SetupPhotonServer);
				NetworkManager.Instance.DiscoverServer();
			}
		}

		// TODO: Comments
		public void Disconnect()
		{
			PhotonNetwork.Disconnect();
		}

		public void CleanNetworkComponent(GameObject go)
		{
			Destroy(go.GetComponent<PhotonSyncPosition>());
			Destroy(go.GetComponent<PhotonTransformView>());
			Destroy(go.GetComponent<PhotonView>());
		}

		///<summary>IsConnected is a method in the INetworkImplementation interface.</summary>
		public bool IsConnected()
		{
			return PhotonNetwork.IsConnectedAndReady;
		}
		#endregion

		#region MatchmakingInterface

		public void JoinMainLobby()
		{
			//If we're still connected to the photon server, join the main Lobby
			if (NetworkManager.Instance.Manager.IsConnected())
			{
				PhotonNetwork.JoinLobby();
			}
		}

		/// <summary>Connect is a method in the INetworkMatchmaking interface.</summary>
		/// <remarks>Create a room open with the same max users as the UserManager and visible to all players of the main lobby.</remarks>
		/// <remarks>If the room is already created  JoinRoom is called.</remarks>
		/// <remarks>OnCreatedRoom from PunCallbackImplementation is called on success and OnCreateRoomFailed on failure.</remarks>
		public void CreateRoom(string roomName)
		{
			if (!IsConnected())
			{
				Debug.Assert(false, "You have to be connected before create a room.");
				return;
			}

			if (!_CachedRoomInfos.ContainsKey(roomName))
			{
				RoomOptions option = new RoomOptions()
				{
					MaxPlayers = (byte)UserManager.Instance.MaxUsers,
					IsOpen = true,
					IsVisible = true
				};

				PhotonNetwork.CreateRoom(roomName, option);
			}

		}

		/// <summary>JoinRoom is a method in the INetworkMatchmaking interface.</summary>
		/// <remarks>Join a room from the main lobby.</remarks>
		/// <remarks>OnJoinedRoom from PunCallbackImplementation is called on success and OnJoinRoomFailed on failure.</remarks>
		public void JoinRoom(string roomName)
		{
			Debug.LogWarning($"Join room : {roomName}.");
			PhotonNetwork.JoinRoom(roomName);
		}

		public void CloseRoom(bool isOpen)
		{
			if (IsInRoom() && UserManager.Instance.LocalUser.UserRole >= EnumUserRole.Actor)
			{
				if (PhotonNetwork.CurrentRoom.PlayerCount >= UserManager.Instance.MaxUsers)
				{
					isOpen = false;
				}

				PhotonNetwork.CurrentRoom.IsOpen = isOpen;
				Debug.Log($"[NETWORK MANAGER] CurrentRoom status -> {PhotonNetwork.CurrentRoom.IsOpen}");
			}
		}

		/// <summary>LeftRoom is a method in the INetworkMatchmaking interface.</summary>
		/// <remarks>Leave the current room.</remarks>
		/// <remarks>OnLeftRoom from PunCallbackImplementation is called.</remarks>
		public void LeftRoom()
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				return;
			}

			PhotonNetwork.LeaveRoom();
		}

		public void KickUser(int networkId)
		{
			if (UserManager.Instance.LocalUser.UserRole < EnumUserRole.Actor || !IsConnected())
			{
				return;
			}

			Player p = PhotonNetwork.CurrentRoom.GetPlayer(networkId);
			Debug.Assert(p != null);
			PhotonNetwork.CloseConnection(p);
		}

		public void InitSyncUserPosition(GameObject user, bool active)
		{
			PhotonSyncPosition psp = user.GetComponent<PhotonSyncPosition>();
			int id = user.GetComponent<User>().NetworkId;

			if (psp == null) // En vrai cette console était pas mal :)
			{
				psp = user.AddComponent<PhotonSyncPosition>();
			}

			psp.SetActive(active);
			psp.SetPhotonViewInfo(id, UserManager.Instance.LocalUser.NetworkId == id);
		}

		/// <summary>IsInRoom is a method in the INetworkMatchmaking interface.</summary>
		public bool IsInRoom()
		{
			return PhotonNetwork.CurrentRoom != null;
		}

		public void ClearRoom()
		{
			_CachedRoomInfos.Clear();
		}

		/// <summary>GetRoomNames is a method in the INetworkMatchmaking interface.</summary>
		/// <remarks>Return all room name from the main Lobby.</remarks>
		public List<string> GetRoomNames()
		{
			return new List<string>(_CachedRoomInfos.Keys);
		}

		public RoomData GetRoomData(string roomName)
		{
			Debug.Assert(_CachedRoomInfos.ContainsKey(roomName));
			return _CachedRoomInfos[roomName];
		}

		#endregion

		#region RemoteActionInterface

		//TODO: Comments
		public void SetupRemoteAction(GameObject obj, Dictionary<string, Action<object[]>> actions)
		{
			PhotonRPCImplementation rpci = obj.GetComponent<PhotonRPCImplementation>();

			if (rpci == null)
			{
				rpci = obj.AddComponent<PhotonRPCImplementation>();
			}
			rpci.AddRemoteAction(actions);
		}

		//TODO: Comments
		public void ExecuteRemoteAction(GameObject obj, string id, params object[] parameters)
		{
			Debug.Assert(obj.GetComponent<PhotonRPCImplementation>() != null, $"There is no PhotonRPCImplementation in {obj.name}");
			if (NetworkManager.Instance.Manager.IsInRoom())
			{
				obj.GetComponent<PhotonRPCImplementation>()?.InvokeActionLocal(id, parameters);
			}
		}

		public void ExecuteRemoteAction(GameObject obj, string id, bool executeOnServerForAll, params object[] parameters)
		{
			if (!executeOnServerForAll)
			{
				ExecuteRemoteAction(obj, id, parameters);
				return;
			}

			Debug.Assert(obj.GetComponent<PhotonRPCImplementation>() != null, $"There is no PhotonRPCImplementation in {obj.name}");
			if (NetworkManager.Instance.Manager.IsInRoom())
			{
				obj.GetComponent<PhotonRPCImplementation>()?.InvokeActionViaServer(id, parameters);
			}
		}

		#endregion
	}
}