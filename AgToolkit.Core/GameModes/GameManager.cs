using System.Collections;
using AgToolkit.AgToolkit.Core.Loader;
using UnityEngine;
using  AgToolkit.AgToolkit.Core.Singleton;

namespace AgToolkit.AgToolkit.Core.GameModes
{
	public class GameManager : Singleton<GameManager>
	{
		[SerializeField]
		private GameModeConfig GameModesConfig = null;

		public GameMode CurrentGameMode { get; private set; }

		protected void Start()
		{
			SceneLoader.Instance.OnBeforeUnload += OnBeforeUnload;
			SceneLoader.Instance.OnAfterLoad += OnAfterLoad;

			ChangeGameMode(GameModesConfig?.FirstGameMode);
		}

		public T GetCurrentGameMode<T>() where T : GameMode
		{
			Debug.Assert(CurrentGameMode != null && CurrentGameMode is T, $"CurrentGameMode {CurrentGameMode?.Id.Name} is not of requested type {typeof(T).Name}");

			return CurrentGameMode as T;
		}

		public void ChangeGameMode(EnumGameMode gameMode)
		{
			Debug.Assert(GameModesConfig != null, $"No GameModeConfig set in GameManager.");

			SceneDesc sceneToLoad = GameModesConfig.GetSceneDesc(gameMode);
			Debug.Assert(sceneToLoad != null, $"No SceneDesc set for GameMode {gameMode.Name}, did you forget to set GameManager entry ?");

			ChangeGameMode(sceneToLoad);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sceneDesc">scriptable object containing the new scene(s) to load, one of them must contain a GameMode </param>
		internal void ChangeGameMode(SceneDesc sceneDesc)
		{
			SceneLoader.Instance.Load(sceneDesc);
		}

		internal void SetGameMode(GameMode gameMode)
		{
			Debug.Assert(CurrentGameMode == null);

			CurrentGameMode = gameMode;
		}

		private IEnumerator OnBeforeUnload()
		{
			yield return CurrentGameMode?.OnUnload();
			CurrentGameMode = null;
		}

		private IEnumerator OnAfterLoad()
		{
			//new game mode should have register in its Awake function
			Debug.Assert(CurrentGameMode != null, "Could not found any GameMode, ensure you have one GameMode component, and that GameMode.Awake is called");

			yield return CurrentGameMode.OnLoad();
		}

	}
}
