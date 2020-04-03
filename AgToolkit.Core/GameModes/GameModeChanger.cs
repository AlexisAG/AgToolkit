using UnityEngine;

namespace AgToolkit.AgToolkit.Core.GameModes
{
	/// <summary>
	/// small component usefull to change a gaemmode from a unityevent
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
