using UnityEngine;

namespace AgToolkit.Core.Pool
{
	public class PoolMember : MonoBehaviour
	{
		internal Pool ParentPool { private get; set; }

		internal bool Available { get; set; } = true;
		internal bool BackToPool { get; set; } = false;

		internal bool DestroyedByPool { private get; set; } = false;

        private void OnDisable()
        {
            if (gameObject.activeSelf) return;
            if (!ParentPool._poolData.IsAutoSendBack) return;

            SendBackToPool();
        }

        /// <summary>
        /// SendBack the gameobject to his pool.
        /// </summary>
        public void SendBackToPool()
		{
			BackToPool = true;
			Available = true;
            //wait one frame (otherwise causes error due to setactive(false) being called the same frame as re parenting)
			Invoke(nameof(DelayedRattached), 0.0001f);
		}

		private void DelayedRattached()
		{
			if (BackToPool) //else pool member has already be taken by pool again
			{
				//attach back to parent pool
				ParentPool.BackToPool(gameObject);
			}
		}

		private void OnDestroy()
		{
			Debug.Assert(DestroyedByPool, $"Pool Object {gameObject.name} destroyed outside of its pool {ParentPool._poolData.PoolId}");
		}
	}
}