using System.Collections;
using AgToolkit.AgToolkit.Core.GameModes;
using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.Core.Loader;
using UnityEngine;

namespace AgToolkit.Core.GameModes
{
	public class GameManager : Singleton<GameManager>
	{
		[SerializeField]
		private GameModeConfig _gameModesConfig = null;

		public GameMode CurrentGameMode { get; private set; }

		protected void Start()
		{
			SceneLoaderManager.Instance.OnBeforeUnload += OnBeforeUnload;
			SceneLoaderManager.Instance.OnAfterLoad += OnAfterLoad;

			ChangeGameMode(_gameModesConfig?.FirstGameMode);
		}

		public T GetCurrentGameMode<T>() where T : GameMode
		{
			Debug.Assert(CurrentGameMode != null && CurrentGameMode is T, $"CurrentGameMode {CurrentGameMode?.Id.Name} is not of requested type {typeof(T).Name}");

			return CurrentGameMode as T;
		}

		public void ChangeGameMode(EnumGameMode gameMode)
		{
			Debug.Assert(_gameModesConfig != null, $"No GameModeConfig set in GameManager.");

			SceneContent sceneToLoad = _gameModesConfig.GetSceneContent(gameMode);
			Debug.Assert(sceneToLoad != null, $"No SceneContent set for GameMode {gameMode.Name}, did you forget to set GameManager entry ?");

			ChangeGameMode(sceneToLoad);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sceneContent">scriptable object containing the new scene(s) to load, one of them must contain a GameMode </param>
		internal void ChangeGameMode(SceneContent sceneContent)
		{
			SceneLoaderManager.Instance.Load(sceneContent);
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
