using AgToolkit.Core.Helper;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.GameModes
{
    [CreateAssetMenu(menuName = "AgToolkit/GameMode/EnumGameMode", fileName = "NewGameModeEnum")]
	public class EnumGameMode : ScriptableObject
	{
		public string Name => name;
	}
}
