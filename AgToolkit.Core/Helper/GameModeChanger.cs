using AgToolkit.AgToolkit.Core.GameModes;
using AgToolkit.Core.GameModes;
using UnityEngine;

namespace AgToolkit.Core.Helper
{
	/// <summary>
	/// small component usefull to change a gamemode from a unityevent
	/// </summary>
	public class GameModeChanger : MonoBehaviour
	{
		public EnumGameMode nextGameMode;

		public void ChangeGameMode()
		{
			Debug.Assert(nextGameMode != null);
			GameManager.Instance.ChangeGameMode(nextGameMode);
		}

	}
}
