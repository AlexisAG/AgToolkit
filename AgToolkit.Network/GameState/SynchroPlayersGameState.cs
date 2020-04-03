using System;
using AgToolkit.AgToolkit.Core.GameModes.GameStates;

namespace AgToolkit.AgToolkit.Network.GameState
{
	[Serializable]
	public class SynchroPlayersData : IGameStateData
	{
		public string TriggerName = "PlayerSynchronized";
	}

	public class BaseSynchroPlayersGameState<T> : GameStateMonoBehaviour<T> where T : SynchroPlayersData
	{
	}

	public class SynchroPlayersGameState : GameStateMonoBehaviour<SynchroPlayersData>
	{
	}
}

