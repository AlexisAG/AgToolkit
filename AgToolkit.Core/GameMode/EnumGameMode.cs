using UnityEngine;

namespace AgToolkit.Core.GameMode
{
    [CreateAssetMenu(menuName = "AgToolkit/GameMode/EnumGameMode", fileName = "NewGameModeEnum")]
	public class EnumGameMode : ScriptableObject
	{
		public string Name => name;
	}
}
