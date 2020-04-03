using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AgToolkit.AgToolkit.Core.Singleton;

namespace AgToolkit.AgToolkit.Core.Loader
{
	public class SceneLoader : Singleton<SceneLoader>
	{

		[SerializeField]
		private SceneReference DefaultLoadingScene = null;

		[SerializeField]
		private List<SceneReference> AdditionalPersistentScenes = new List<SceneReference>();

		public float MinLoadTimeSec = 1.0f;

		//events
		public event Func<IEnumerator> OnBeforeUnload = null;
		public event Func<IEnumerator> OnAfterLoad = null;

		public event Func<IEnumerator> OnFadeIn = null;
		public event Func<IEnumerator> OnFadeOut = null;

		private SceneDesc NextSceneDesc = null;

		private readonly Stack<string> ScenesToUnload = new Stack<string>();

		private bool IsParsingPersistentScenesList = false;

		public void AddPersistentSceneToLoad(SceneReference scene)
		{
			if (IsParsingPersistentScenesList)
			{
				Debug.LogWarning("[SceneLoader] You should not add a persistent scene while loading another one, unexpected behavior.");
			}

			AdditionalPersistentScenes.Add(scene);
		}

		/// <summary>
		/// Start loading next scene(s) and unloading previous ones
		/// </summary>
		/// <param name="sceneDesc">next SceneDesc asset to load, or null to use <see cref="NextSceneDesc"</see> </param>
		public void Load(SceneDesc sceneDesc)
		{
			if (sceneDesc != null)
			{
				NextSceneDesc = sceneDesc;
			}
			StartCoroutine(DoLoad());
		}

		private IEnumerator DoLoad()
		{
			//load loading scene
			Debug.Log($"[Loading] start");
			//use loading scene provided in desc, or fallback to default if null
			string loadingScenePath = NextSceneDesc?.LoadingScene?.ScenePath;
			if (string.IsNullOrEmpty(loadingScenePath))
			{
				loadingScenePath = DefaultLoadingScene?.ScenePath;
			}
			if (string.IsNullOrEmpty(loadingScenePath))
			{
				Debug.LogWarning("[SceneLoader] No Default Loading Scene defined, consider addding one.");
			}
			else
			{
				AsyncOperation loadLoadingOp = SceneManager.LoadSceneAsync(loadingScenePath, LoadSceneMode.Additive);
				yield return new WaitUntil(() => loadLoadingOp == null || loadLoadingOp.isDone);
			}

			yield return InvokeActions(OnFadeIn, true);

			//start loading timer
			float startLoadingTime = Time.realtimeSinceStartup;

			//unload previous scenes
			yield return InvokeActions(OnBeforeUnload);

			while (ScenesToUnload.Count > 0)
			{
				string scene = ScenesToUnload.Pop();
				Debug.Log($"[Loading] unloading {scene}");
				AsyncOperation unloadSceneOp = SceneManager.UnloadSceneAsync(scene);
				yield return new WaitUntil(() => unloadSceneOp == null || unloadSceneOp.isDone);
			}
			ScenesToUnload.Clear();

			//load persistent scenes (they will never be unloaded)
			IsParsingPersistentScenesList = true;
			for (int i = 0; i < AdditionalPersistentScenes.Count; ++i)
			{
				string scene = AdditionalPersistentScenes[i].ScenePath;
				Debug.Log($"[Loading] loading {scene} (persistent)");
				AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
				yield return new WaitUntil(() => loadSceneOp == null || loadSceneOp.isDone);
			}
			// clear array, to be able to push new persistent scenes to load later on
			AdditionalPersistentScenes.Clear();
			IsParsingPersistentScenesList = false;

			//then load new ones
			for (int i = 0; i < NextSceneDesc.ContentScenes.Length; ++i)
			{
				string scene = NextSceneDesc.ContentScenes[i].ScenePath;
				Debug.Log($"[Loading] loading {scene}");
				AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
				yield return new WaitUntil(() => loadSceneOp == null || loadSceneOp.isDone);

				ScenesToUnload.Push(scene);
			}
			//lighting and active scene
			string lightScene = NextSceneDesc.LightingScene?.ScenePath;
			if (!string.IsNullOrEmpty(lightScene))
			{
				Debug.Log($"[Loading] loading {lightScene}");
				AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(lightScene, LoadSceneMode.Additive);
				yield return new WaitUntil(() => loadSceneOp == null || loadSceneOp.isDone);

				ScenesToUnload.Push(lightScene);

				SceneManager.SetActiveScene(SceneManager.GetSceneByPath(lightScene));
			}
			else if (NextSceneDesc.ContentScenes.Length > 0)
			{
				//default active scene to first content scene
				SceneManager.SetActiveScene(SceneManager.GetSceneByPath(NextSceneDesc.ContentScenes[0].ScenePath));
			}

			yield return InvokeActions(OnAfterLoad);

			Debug.Log(string.Format("[Loading] took {0:00} ms", (Time.realtimeSinceStartup - startLoadingTime) * 1000));
			//wait the minimum time
			yield return new WaitUntil(() => MinLoadTimeSec <= (Time.realtimeSinceStartup - startLoadingTime));

			yield return InvokeActions(OnFadeOut, true);

			//finally unload loading scene
			if (!string.IsNullOrEmpty(loadingScenePath))
			{
				AsyncOperation unloadLoadingOp = SceneManager.UnloadSceneAsync(loadingScenePath);
				yield return new WaitUntil(() => unloadLoadingOp == null || unloadLoadingOp.isDone);
			}


			Debug.Log($"[Loading] end");
		}

		private IEnumerator InvokeActions(Func<IEnumerator> enumeratorEvent, bool parallel = false)
		{
			if (enumeratorEvent != null)
			{
				if (parallel)
				{
					List<Coroutine> allCoroutines = new List<Coroutine>();

					//call for each registered by hand, otherwise only last registered is called
					foreach (Delegate del in enumeratorEvent.GetInvocationList())
					{
						allCoroutines.Add(StartCoroutine((IEnumerator)del.DynamicInvoke()));
					}

					foreach (Coroutine c in allCoroutines)
					{
						yield return c;
					}
				}
				else
				{
					foreach (Delegate del in enumeratorEvent.GetInvocationList())
					{
						yield return StartCoroutine((IEnumerator)del.DynamicInvoke());
					}
				}
			}
		}
	}
}