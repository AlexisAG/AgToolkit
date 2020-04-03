using System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace AgToolkit.AgToolkit.Core.Loader
{
	[Serializable]
	public sealed class SceneReference : ISerializationCallbackReceiver
	{

		[SerializeField]
		private UnityEngine.Object sceneAsset; // hidden by the drawer

#if UNITY_EDITOR
		private bool IsValidSceneAsset
		{
			get
			{
				if (sceneAsset == null)
				{
					return true;
				}

				return sceneAsset.GetType().Equals(typeof(SceneAsset));
			}
		}
#endif

		[SerializeField]
		private string scenePath; // hidden by the drawer
								  // Use this when you want to actually have the scene path
		public string ScenePath
		{
			get =>
#if UNITY_EDITOR
				// In editor we always use the asset's path
				GetScenePathFromAsset();
#else
				// At runtime we rely on the stored path value which we assume was serialized correctly at build time.
				// See OnBeforeSerialize and OnAfterDeserialize
				scenePath;
#endif

			set
			{
				scenePath = value;
#if UNITY_EDITOR
				sceneAsset = GetSceneAssetFromPath();
#endif
			}
		}

		public static implicit operator string(SceneReference sceneReference)
		{
			return sceneReference.ScenePath;
		}

		#region ISerializationCallbackReceiver Members

		// Called to prepare this data for serialization. Stubbed out when not in editor.
		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			HandleBeforeSerialize();
#endif
		}

		// Called to set up data for deserialization. Stubbed out when not in editor.
		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			// We sadly cannot touch assetdatabase during serialization, so defer by a bit.
			EditorApplication.update += HandleAfterDeserialize;
#endif
		}

		#endregion

#if UNITY_EDITOR
		private SceneAsset GetSceneAssetFromPath()
		{
			if (string.IsNullOrEmpty(scenePath))
			{
				return null;
			}

			return AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
		}

		private string GetScenePathFromAsset()
		{
			if (sceneAsset == null)
			{
				return string.Empty;
			}

			return AssetDatabase.GetAssetPath(sceneAsset);
		}

		private void HandleBeforeSerialize()
		{
			// Asset is invalid but have Path to try and recover from
			if (IsValidSceneAsset == false && string.IsNullOrEmpty(scenePath) == false)
			{
				sceneAsset = GetSceneAssetFromPath();
				if (sceneAsset == null)
				{
					scenePath = string.Empty;
				}

				UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
			}
			// Asset takes precendence and overwrites Path
			else
			{
				scenePath = GetScenePathFromAsset();
			}
		}

		private void HandleAfterDeserialize()
		{
			EditorApplication.update -= HandleAfterDeserialize;
			// Asset is valid, don't do anything - Path will always be set based on it when it matters
			if (IsValidSceneAsset)
			{
				return;
			}

			// Asset is invalid but have path to try and recover from
			if (string.IsNullOrEmpty(scenePath) == false)
			{
				sceneAsset = GetSceneAssetFromPath();
				// No asset found, path was invalid. Make sure we don't carry over the old invalid path
				if (sceneAsset == null)
				{
					scenePath = string.Empty;
				}

				if (Application.isPlaying == false)
				{
					UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
				}
			}
		}
#endif

	}
}