using System;

namespace AgToolkit.AgToolkit.Core.GameModes.GameStates
{
	[Serializable]
	public class ExitData : IGameStateData
	{
		public EnumGameMode NextGameMode;
	}


	/// <summary>
	/// a generic gamestate to change to another gamemode
	/// in case you need to cahnge to pultiple gamemodes, just change the ExitData.NextGameMode by code before triggering this gamestate
	/// </summary>
	public class ExitGameState : GameStateMonoBehaviour<ExitData>
	{

	}
}