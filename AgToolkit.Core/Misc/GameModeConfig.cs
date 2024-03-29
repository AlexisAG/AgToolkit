using System.Collections.Generic;
using System.Linq;
using AgToolkit.Core.GameMode;
using UnityEngine;

namespace AgToolkit.Core.Misc
{
	[CreateAssetMenu(menuName = "AgToolkit/GameMode/GameModeConfig", fileName = "NewGameModeConfig")]
	public class GameModeConfig : ScriptableObject
	{
		[SerializeField]
		private EnumGameMode _firstGameMode = null;

        [SerializeField]
		private List<GameModeSceneContentPair> _gameModeContentPairs = new List<GameModeSceneContentPair> { new GameModeSceneContentPair { } };
        public EnumGameMode FirstGameMode => _firstGameMode;

        public SceneContent GetSceneContent(EnumGameMode gameMode)
		{
			Debug.Assert(_gameModeContentPairs.Any(p => gameMode == p.GameMode), $"No entry in GameModeConfig for {gameMode.Name}");
			return _gameModeContentPairs.Single(p => gameMode == p.GameMode)?.SceneContent;
		}

	}
}