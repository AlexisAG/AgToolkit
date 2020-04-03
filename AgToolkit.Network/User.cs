using UnityEngine;

namespace AgToolkit.AgToolkit.Network
{
	public enum EnumUserRole
	{
		Spectator,
		Actor,
		Master
	};

	public enum EnumUserType
	{
		Local,
		Network
	};

	public class User : MonoBehaviour
	{
		private static int _localIds = 0;

		[SerializeField]
		private EnumUserRole _UserRole;
		[SerializeField]
		private EnumUserType _UserType;

		private GameObject _UserIndicator = null;

		public EnumUserRole UserRole => _UserRole;

		public EnumUserType UserType => _UserType;

		public string CurrentRoom = null;
		public int LocalId { get; } = _localIds++;
		public int NetworkId { get; private set; } = -1;
		public bool IsReady = false;

		private void RemoveIndicator()
		{
			_UserIndicator.SetActive(false);
			//_UserIndicator.GetComponentInChildren<TextMeshPro>().SetText("");
			_UserIndicator = null;
		}

		public void SetInfo(EnumUserRole role, EnumUserType type, int networkId = -1)
		{
			_UserRole = role;
			_UserType = type;

			if (_UserType == EnumUserType.Network)
			{
				Debug.Assert(networkId >= 0, $"NetworkID have to be initialized and greater than 0 for User {LocalId}.");
				NetworkId = networkId;

				// If player is in a room (0 = in Lobby)
				// Use the networkID and not the NetworkManager because i don't want to link User & networkManager
				if (NetworkId >= 1 && _UserIndicator == null)
				{
					_UserIndicator = UserManager.Instance.GetUserIndicator(UserRole);
					_UserIndicator.transform.SetParent(gameObject.transform, false);
					//_UserIndicator.GetComponentInChildren<TextMeshPro>().SetText(SystemInfo.deviceName);
					_UserIndicator.SetActive(true);
				}
			}

			if (NetworkId < 1 && _UserIndicator != null)
			{
				RemoveIndicator();
			}
		}

		public void ResetInfo()
		{
			SetInfo(EnumUserRole.Spectator, EnumUserType.Local);
		}
	}
}

