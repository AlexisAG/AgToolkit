using System;
using System.Collections;
using System.Collections.Generic;
using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.Core.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AgToolkit.Core.Loader
{
	public class SceneLoaderManager : Singleton<SceneLoaderManager>
	{
        [SerializeField]
		private SceneReference _defaultLoadingScene = null;

		[SerializeField]
		private List<SceneReference> _additionalPersistentScenes = new List<SceneReference>();

		public float _minLoadTimeSec = 1.0f;

		//events
		public event Func<IEnumerator> OnBeforeUnload = null;
		public event Func<IEnumerator> OnAfterLoad = null;

		public event Func<IEnumerator> OnFadeIn = null;
		public event Func<IEnumerator> OnFadeOut = null;

		private SceneContent _nextSceneContent = null;

		private readonly Stack<string> _scenesToUnload = new Stack<string>();

		private bool _isParsingPersistentScenesList = false;

        /// <summary>
        /// Add a persistent scene 
        /// </summary>
        public void AddPersistentSceneToLoad(SceneReference scene)
		{
			if (_isParsingPersistentScenesList)
			{
				Debug.LogWarning($"[{this.GetType().Name}] You should not add a persistent scene while loading another one, unexpected behavior.");
			}

			_additionalPersistentScenes.Add(scene);
		}

		/// <summary>
		/// Start loading next scene(s) and unloading previous ones
		/// </summary>
		/// <param name="sceneContent">next SceneContent asset to load, or null to use <see cref="_nextSceneContent"</see> </param>
		public void Load(SceneContent sceneContent)
		{
			if (sceneContent != null)
			{
				_nextSceneContent = sceneContent;
			}
			CoroutineManager.Instance.StartCoroutine(DoLoad());
		}

		private IEnumerator DoLoad()
        {
            Debug.Log($"[Loading] start");
            yield return DisplayLoadingScene();
            yield return InvokeActions(OnFadeIn, true);

			//start loading timer
			float startLoadingTime = Time.realtimeSinceStartup;

            yield return UnLoadPreviousScene();

            //load persistent scenes (they will never be unloaded)
            _isParsingPersistentScenesList = true;
            yield return LoadMultipleScenes(_additionalPersistentScenes.ToArray(), true);

			// clear array, to be able to push new persistent scenes to load later on
			_additionalPersistentScenes.Clear();
			_isParsingPersistentScenesList = false;

			//then load new ones
            yield return LoadMultipleScenes(_nextSceneContent.ContentScenes, false);
            
			//lighting and active scene
			string lightScene = _nextSceneContent.LightingScene?.ScenePath;
			if (!string.IsNullOrEmpty(lightScene))
			{
				Debug.Log($"[{this.GetType().Name}] loading {lightScene}");
                yield return LoadScene(lightScene, false);

				SceneManager.SetActiveScene(SceneManager.GetSceneByPath(lightScene));
			}
			else if (_nextSceneContent.ContentScenes.Length > 0)
			{
				//default active scene to first content scene
				SceneManager.SetActiveScene(SceneManager.GetSceneByPath(_nextSceneContent.ContentScenes[0].ScenePath));
			}

			yield return InvokeActions(OnAfterLoad);

			//wait the minimum time
			yield return new WaitUntil(() => _minLoadTimeSec <= (Time.realtimeSinceStartup - startLoadingTime));
			Debug.Log(string.Format("[Loading] took {0:00} ms", (Time.realtimeSinceStartup - startLoadingTime) * 1000));

			yield return InvokeActions(OnFadeOut, true);

			//finally unload loading scene
			if (!string.IsNullOrEmpty(_nextSceneContent?.LoadingScene?.ScenePath))
            {
                yield return UnLoadScene(_nextSceneContent?.LoadingScene?.ScenePath);
            }
            else if (!string.IsNullOrEmpty(_defaultLoadingScene?.ScenePath))
            {
                yield return UnLoadScene(_defaultLoadingScene?.ScenePath);
            }
        }

        private IEnumerator UnLoadScene(string scenePath)
        {
            AsyncOperation unloadLoadingOp = SceneManager.UnloadSceneAsync(scenePath);
            yield return new WaitUntil(() => unloadLoadingOp == null || unloadLoadingOp.isDone);
        }

        private IEnumerator UnLoadMultipleScenes(SceneReference[] scenes)
        {
            foreach (SceneReference sr in scenes) {
                string scene = sr.ScenePath;
                Debug.Log($"[{this.GetType().Name}] loading {scene}");
                yield return UnLoadScene(scene);
            }
        }

        private IEnumerator LoadScene(string scenePath, bool isPersistantScene)
        {
            AsyncOperation loadLoadingOp = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            if (!isPersistantScene)
            {
                _scenesToUnload.Push(scenePath);
            }
            yield return new WaitUntil(() => loadLoadingOp == null || loadLoadingOp.isDone);
        }

        private IEnumerator LoadMultipleScenes(SceneReference[] scenes, bool areContentScenes)
        {
            foreach (SceneReference sr in scenes)
            {
                string scene = sr.ScenePath;
                Debug.Log($"[{this.GetType().Name}] loading {scene}");
                yield return LoadScene(scene, areContentScenes);
            }
        }

        private IEnumerator DisplayLoadingScene()
        {
            //use loading scene provided in desc, or fallback to default if null
            string loadingScenePath = _nextSceneContent?.LoadingScene?.ScenePath;
            if (string.IsNullOrEmpty(loadingScenePath)) {
                loadingScenePath = _defaultLoadingScene?.ScenePath;
            }

            if (string.IsNullOrEmpty(loadingScenePath)) {
                Debug.LogWarning($"[{this.GetType().Name}] No Default Loading Scene defined, consider addding one.");
            }
            else
            {
                yield return LoadScene(loadingScenePath, true);
            }
        }

        private IEnumerator UnLoadPreviousScene()
        {
            //unload previous scenes
            yield return InvokeActions(OnBeforeUnload);

            while (_scenesToUnload.Count > 0) {
                string scene = _scenesToUnload.Pop();
                Debug.Log($"[{this.GetType().Name}] unloading {scene}");
                yield return UnLoadScene(scene);
            }
            _scenesToUnload.Clear();
        }

		private IEnumerator InvokeActions(Func<IEnumerator> enumeratorEvent, bool parallel = false)
        {
            if (enumeratorEvent == null) yield break;

            if (parallel) {
                List<Coroutine> allCoroutines = new List<Coroutine>();

                //call for each registered by hand, otherwise only last registered is called
                foreach (Delegate del in enumeratorEvent.GetInvocationList()) {
                    allCoroutines.Add(CoroutineManager.Instance.StartCoroutine((IEnumerator)del.DynamicInvoke()));
                }
                foreach (Coroutine c in allCoroutines) {
                    yield return c;
                }
            }
            else {
                foreach (Delegate del in enumeratorEvent.GetInvocationList()) {
                    yield return CoroutineManager.Instance.StartCoroutine((IEnumerator)del.DynamicInvoke());
                }
            }
        }
	}
}