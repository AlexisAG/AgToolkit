using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Loader
{
	[CreateAssetMenu(menuName = "AgToolkit/Scene Desc", fileName = "NewSceneDesc")]
	public class SceneDesc : ExpandableScriptableObject
	{
		public SceneReference LoadingScene;

		public SceneReference[] ContentScenes = new SceneReference[1];

		public SceneReference LightingScene;
	}
}