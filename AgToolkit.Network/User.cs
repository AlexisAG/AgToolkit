using UnityEngine;

namespace AgToolkit.Network
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

		public EnumUserRole UserRole => _UserRole;

		public EnumUserType UserType => _UserType;

		public string CurrentRoom = null;
		public int LocalId { get; } = _localIds++;
		public int NetworkId { get; private set; } = -1;
		public bool IsReady = false;

		public void SetInfo(EnumUserRole role, EnumUserType type, int networkId = -1)
		{
			_UserRole = role;
			_UserType = type;

			if (_UserType == EnumUserType.Network)
			{
				Debug.Assert(networkId >= 0, $"NetworkID have to be initialized and greater than 0 for User {LocalId}.");
				NetworkId = networkId;
			}
		}

		public void ResetInfo()
		{
			SetInfo(EnumUserRole.Spectator, EnumUserType.Local);
			NetworkId = -1;
		}
	}
}

