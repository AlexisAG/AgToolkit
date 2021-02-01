using System;
using AgToolkit.Core.GameModes.GameStates;

namespace AgToolkit.Network
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

