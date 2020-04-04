using System.Collections.Generic;
using System.Linq;
using AgToolkit.Core.Helper;
using AgToolkit.Core.Loader;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.GameModes
{
	[CreateAssetMenu(menuName = "AgToolkit/Game Mode Config", fileName = "NewGameModeConfig")]
	public class GameModeConfig : ScriptableObject
	{
		[SerializeField]
		private EnumGameMode _firstGameMode = null;

		public EnumGameMode FirstGameMode => _firstGameMode;

		[SerializeField]
		private List<GameModeSceneDescPair> gameModeDesc = new List<GameModeSceneDescPair> { new GameModeSceneDescPair { } };

		public SceneContent GetSceneDesc(EnumGameMode gameMode)
		{
			Debug.Assert(gameModeDesc.Any(p => gameMode == p.GameMode), $"No entry in GameModeConfig for {gameMode.Name}");
			return gameModeDesc.Single(p => gameMode == p.GameMode)?._sceneContent;
		}

	}
}