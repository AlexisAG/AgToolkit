using UnityEngine;

namespace AgToolkit.Core.GameMode
{

	/// <summary>
	/// For Each GameStateMachineBehaviour in a AnimatorController, you can add a GameStateMonoBehaviour to reference data from scene
	/// </summary>
	/// <typeparam name="T"></typeparam>

	public abstract class GameStateMonoBehaviour<T> : BaseGameStateMonoBehaviour where T : IGameStateData
	{
		[SerializeField]
		[Tooltip("Set reference to scene here, use it in corresponding GameStateMachineBehaviour.")]
		protected T data;

		protected virtual void Awake()
		{
			Debug.Assert(GetComponent<Animator>().GetBehaviours<GameStateMachineBehaviour<T>>().Length == 1, $"Expecting 1 {typeof(GameStateMachineBehaviour<T>).Name}, got {GetComponent<Animator>().GetBehaviours<GameStateMachineBehaviour<T>>().Length}");
			//reference data to be accessible from state
			GameStateMachineBehaviour<T> correspondingState = GetComponent<Animator>().GetBehaviour<GameStateMachineBehaviour<T>>();

			correspondingState.Data = data;
		}

	}
}