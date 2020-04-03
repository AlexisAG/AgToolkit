using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Pool
{
    /// <summary>
    /// Attach this script on a PoolMember, this class allow to display / hide the gameobject.
    /// </summary>
    public class VisualObjectController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _visualGameObject = null;
        public GameObject VisualGameObject => _visualGameObject;

        public void DisplayVisualObject(bool display)
        {
            Debug.Assert(VisualGameObject != null, $"There is no visual gameobject specified in {gameObject.name}.");

            VisualGameObject.SetActive(display);
        }

    }
}
