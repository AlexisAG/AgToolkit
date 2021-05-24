using System.Collections;
using AgToolkit.Core.DesignPattern;
using AgToolkit.Core.Loader;
using UnityEngine;
using AgToolkit.Core.Helper;
using AgToolkit.Core.GameMode;

namespace AgToolkit.Core.Manager
{
	public class GameManager : Singleton<GameManager>
	{
		[SerializeField]
		private GameModeConfig _gameModesConfig = null;
        private SceneContent _currentSceneContent = null;

		public GameMode.GameMode CurrentGameMode { get; private set; }
        public SceneContent CurrentSceneContent { get; private set; }

        protected void Start()
		{
			SceneLoaderManager.Instance.OnBeforeUnload += OnBeforeUnload;
			SceneLoaderManager.Instance.OnAfterLoad += OnAfterLoad;

			ChangeGameMode(_gameModesConfig?.FirstGameMode);
		}

		public T GetCurrentGameMode<T>() where T : GameMode.GameMode
		{
			Debug.Assert(CurrentGameMode != null && CurrentGameMode is T, $"CurrentGameMode {CurrentGameMode?.Id.Name} is not of requested type {typeof(T).Name}");

			return CurrentGameMode as T;
		}

		public void ChangeGameMode(EnumGameMode gameMode)
		{
			Debug.Assert(_gameModesConfig != null, $"No GameModeConfig set in GameManager.");

            CurrentSceneContent = _gameModesConfig.GetSceneContent(gameMode);
			Debug.Assert(CurrentSceneContent != null, $"No SceneContent set for GameMode {gameMode.Name}, did you forget to set GameManager entry ?");

			ChangeGameMode(CurrentSceneContent);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sceneContent">scriptable object containing the new scene(s) to load, one of them must contain a GameMode </param>
		internal void ChangeGameMode(SceneContent sceneContent)
		{
			SceneLoaderManager.Instance.Load(sceneContent);
		}

		internal void SetGameMode(GameMode.GameMode gameMode)
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
			Debug.Assert(CurrentGameMode != null, "Could not found any GameMode, ensure you have one active GameMode component in your scene");
			yield return CurrentGameMode.OnLoad();
		}

	}
}
