using System;
using System.Collections;
using System.Collections.Generic;
using AgToolkit.Core.DesignPattern.Singleton;
using AgToolkit.Core.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace AgToolkit.Core.Loader
{
	public class SceneLoaderManager : Singleton<SceneLoaderManager>
	{
        [FormerlySerializedAs("_defaultLoadingScene")] [SerializeField]
		private SceneReference _DefaultLoadingScene = null;

		[FormerlySerializedAs("_additionalPersistentScenes")] [SerializeField]
		private List<SceneReference> _AdditionalPersistentScenes = new List<SceneReference>();

		[FormerlySerializedAs("_minLoadTimeSec")]
        public float MinLoadTimeSec = 1.0f;

		//events
		public event Func<IEnumerator> OnBeforeUnload = null;
		public event Func<IEnumerator> OnAfterLoad = null;

		public event Func<IEnumerator> OnFadeIn = null;
		public event Func<IEnumerator> OnFadeOut = null;

		private SceneContent _NextSceneContent = null;

		private readonly Stack<string> _ScenesToUnload = new Stack<string>();

		private bool _IsParsingPersistentScenesList = false;

        /// <summary>
        /// Add a persistent scene 
        /// </summary>
        public void AddPersistentSceneToLoad(SceneReference scene)
		{
			if (_IsParsingPersistentScenesList)
			{
				Debug.LogWarning($"[{this.GetType().Name}] You should not add a persistent scene while loading another one, unexpected behavior.");
			}

			_AdditionalPersistentScenes.Add(scene);
		}

		/// <summary>
		/// Start loading next scene(s) and unloading previous ones
		/// </summary>
		/// <param name="sceneContent">next SceneContent asset to load, or null to use <see cref="_NextSceneContent"</see> </param>
		public void Load(SceneContent sceneContent)
		{
			if (sceneContent != null)
			{
				_NextSceneContent = sceneContent;
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
            _IsParsingPersistentScenesList = true;
            yield return LoadMultipleScenes(_AdditionalPersistentScenes.ToArray(), true);

			// clear array, to be able to push new persistent scenes to load later on
			_AdditionalPersistentScenes.Clear();
			_IsParsingPersistentScenesList = false;

			//then load new ones
            yield return LoadMultipleScenes(_NextSceneContent.ContentScenes, false);
            
			//lighting and active scene
			string lightScene = _NextSceneContent.LightingScene?.ScenePath;
			if (!string.IsNullOrEmpty(lightScene))
			{
				Debug.Log($"[{this.GetType().Name}] loading {lightScene}");
                yield return LoadScene(lightScene, false);

				SceneManager.SetActiveScene(SceneManager.GetSceneByPath(lightScene));
			}
			else if (_NextSceneContent.ContentScenes.Length > 0)
			{
				//default active scene to first content scene
				SceneManager.SetActiveScene(SceneManager.GetSceneByPath(_NextSceneContent.ContentScenes[0].ScenePath));
			}

			yield return InvokeActions(OnAfterLoad);

			//wait the minimum time
			yield return new WaitUntil(() => MinLoadTimeSec <= (Time.realtimeSinceStartup - startLoadingTime));
			Debug.Log(string.Format("[Loading] took {0:00} ms", (Time.realtimeSinceStartup - startLoadingTime) * 1000));

			yield return InvokeActions(OnFadeOut, true);

			//finally unload loading scene
			if (!string.IsNullOrEmpty(_NextSceneContent?.LoadingScene?.ScenePath))
            {
                yield return UnLoadScene(_NextSceneContent?.LoadingScene?.ScenePath);
            }
            else if (!string.IsNullOrEmpty(_DefaultLoadingScene?.ScenePath))
            {
                yield return UnLoadScene(_DefaultLoadingScene?.ScenePath);
            }
        }

        private IEnumerator UnLoadScene(string scenePath)
        {
            AsyncOperation unloadLoadingOp = SceneManager.UnloadSceneAsync(scenePath);
            yield return new WaitUntil(() => unloadLoadingOp == null || unloadLoadingOp.isDone);
        }

        private IEnumerator UnLoadMultipleScenes(IEnumerable<SceneReference> scenes)
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
                _ScenesToUnload.Push(scenePath);
            }
            yield return new WaitUntil(() => loadLoadingOp == null || loadLoadingOp.isDone);
        }

        private IEnumerator LoadMultipleScenes(IEnumerable<SceneReference> scenes, bool areContentScenes)
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
            string loadingScenePath = _NextSceneContent?.LoadingScene?.ScenePath;
            if (string.IsNullOrEmpty(loadingScenePath)) {
                loadingScenePath = _DefaultLoadingScene?.ScenePath;
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

            while (_ScenesToUnload.Count > 0) {
                string scene = _ScenesToUnload.Pop();
                Debug.Log($"[{this.GetType().Name}] unloading {scene}");
                yield return UnLoadScene(scene);
            }
            _ScenesToUnload.Clear();
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