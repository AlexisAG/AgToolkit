using UnityEngine;

namespace AgToolkit.Core.GameMode
{
	/// <summary>
	/// Base State Behaviour class for all animator controller states that are used as GameStates
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class GameStateMachineBehaviour<T> : StateMachineBehaviour, IGameState<T> where T : IGameStateData
	{
        [SerializeField]
		[Tooltip("Set values in corresponding BaseStateMonoBehaviour in scene")]
		private T data;

		/// <summary>
		/// data wrapper, reference set by StateMonoBehaviour on Awake
		/// </summary>
		public T Data { protected get => data; set => data = value; }
    }
}