using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.Core.Helper;
using AgToolkit.Core.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgToolkit.Network
{
	public class UserManager : Singleton<UserManager>
	{
		[SerializeField]
		private GameObject _UserPrefab = null;
		[SerializeField]
		private int _MaxUsers = 5;

		public readonly List<User> Users = new List<User>();

		public User LocalUser { get; private set; } = null;
		public int MaxUsers => _MaxUsers;

		protected override void Awake()
		{
			base.Awake();
			CoroutineManager.Instance.StartCoroutine(Init());
		}

		private IEnumerator Init()
		{
			Debug.Assert(_UserPrefab != null, "There is no UserPrefab in UserManager.");
			yield return PoolManager.Instance.CreatePool(new PoolData(_UserPrefab.name, _UserPrefab, _MaxUsers));

			// Register LocalUser
			Register(EnumUserRole.Actor, EnumUserType.Local);
			LocalUser = Users[0];
			LocalUser.transform.SetParent(Camera.main.transform, false);
		}

		private void Start()
		{
			Dictionary<string, Action<object[]>> actions = new Dictionary<string, Action<object[]>>
			{
				{nameof(SetReadyAction), SetReadyAction}
			};
			NetworkManager.Instance.Manager.SetupRemoteAction(gameObject, actions);
		}

		// TODO: comment (args)
		private void SetReadyAction(object[] args)
		{
			// TODO: exception
			int userNetworkId = (int)args[0];
			bool userStatus = (bool)args[1];

			Debug.LogWarning($"[NETWORK] NetworkID -> {userNetworkId}. Status -> {userStatus}.");
			User u = GetUser(userNetworkId);
			u.IsReady = userStatus;
		}

		private User GetUser(int networkId)
		{
			foreach (User u in Users)
			{
				if (u.NetworkId == networkId)
				{
					return u;
				}
			}

			return null;
		}

		public void UnRegister(int networkId)
		{
			User user = GetUser(networkId);
			user.ResetInfo();
			user.gameObject.SetActive(false);

			if (!Users.Remove(user))
			{
				Debug.Assert(false, $"There is no user registered with ID: {user.LocalId} in the userManager");
			}
		}

		public void Register(EnumUserRole role, EnumUserType type, int idNetwork = -1)
		{
			Debug.Assert(GetUser(idNetwork) == null, $"User {idNetwork} already exist.");

			// Create current user
			User user = PoolManager.Instance.GetPooledObject(_UserPrefab.name).GetComponent<User>();
			user.SetInfo(role, type, idNetwork);
			user.gameObject.SetActive(true);

			// Register the new user
			foreach (User u in Users)
			{
				if (u.LocalId == user.LocalId)
				{
					Debug.Assert(false, $"User {user.LocalId} is already registered in UserManager.");
					return;
				}
			}

			Debug.Log($"[NETWORK] User {user.NetworkId} registered.");
			Users.Add(user);
		}

		public bool AreReady()
		{
			foreach (User u in Users)
			{
				if (!u.IsReady)
				{
					Debug.LogWarning($"[NETWORK] User {u.NetworkId} with {u.UserRole} role is not ready.");
					return false;
				}
			}

			return true;
		}

		public List<User> GetUsersNotReady()
		{
			List<User> userList = new List<User>();

			foreach (User u in Users)
			{
				if (!u.IsReady)
				{
					userList.Add(u);
				}
			}

			return userList;
		}

		public void SendReadyStatus()
		{
			Debug.Log($"[NETWORK] Send info about {LocalUser.NetworkId}");

			if (NetworkManager.Instance.Manager.IsConnected() && NetworkManager.Instance.Manager.IsInRoom())
			{
				NetworkManager.Instance.Manager.ExecuteRemoteAction(gameObject, nameof(SetReadyAction), LocalUser.NetworkId, LocalUser.IsReady);
			}
		}

	}
}
