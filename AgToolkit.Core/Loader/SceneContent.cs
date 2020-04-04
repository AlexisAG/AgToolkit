using UnityEngine;

namespace AgToolkit.Core.Loader
{
	[CreateAssetMenu(menuName = "AgToolkit/Scene Content", fileName = "NewSceneContent")]
	public class SceneContent : ScriptableObject
	{
		public SceneReference LoadingScene;

		public SceneReference[] ContentScenes = new SceneReference[1];

		public SceneReference LightingScene;
	}
}