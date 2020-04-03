
namespace AgToolkit.AgToolkit.Core.GameModes.GameStates
{
	/// <summary>
	/// Serializable Data class used to bridge between GameStateMonoBehaviour in scene and GameStateMachineBehaviour in AnimatorController
	/// </summary>
	public interface IGameStateData
	{

	}

	/// <summary>
	/// use this version of StateData when you need to quickly access parent GameMode from your gamestate
	/// </summary>
	/// <typeparam name="G"></typeparam>
	public abstract class GameStateData<G> : IGameStateData where G : GameMode
	{
		public G ParentGameMode => GameManager.Instance.GetCurrentGameMode<G>();
	}

	/// <summary>
	/// interface for GameStateMachineBehaviour that uses GameStateData
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IGameState<T> where T : IGameStateData
	{
		T Data { set; }
	}
}