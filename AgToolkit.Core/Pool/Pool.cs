using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Pool
{

	[Serializable]
	public struct PoolData
	{
		[SerializeField]
		internal string poolId;
		[SerializeField]
		internal GameObject prefab;
		[SerializeField]
		internal int amount;
		[SerializeField]
		internal bool expandable;
		[SerializeField, Tooltip("Object is sent back to pool automatically on disable if true")]
		internal bool autoSendBack;

		public string PoolId => poolId;

		public bool AutoSendBack => autoSendBack;

		public PoolData(string identifier, GameObject prefabToPool, int amountToPool, bool expandablePool = false, bool autoSendBackMembers = true)
		{
			poolId = identifier;
			prefab = prefabToPool;
			amount = amountToPool;
			expandable = expandablePool;
			autoSendBack = autoSendBackMembers;
		}

	}

	[Serializable]
	public class Pool
	{

		[SerializeField]
		internal PoolData poolData;

		private GameObject poolParent = null;
		private List<GameObject> pooledObjects = new List<GameObject>();

		internal GameObject GetOne()
		{
			for (int i = 0; i < pooledObjects.Count; i++)
			{
				PoolMember member = pooledObjects[i].GetComponent<PoolMember>();
				if (!pooledObjects[i].activeSelf && member.Available)
				{
					member.BackToPool = false; //cancel back to pool if disabled same frame
					member.Available = false;
					return pooledObjects[i];
				}
			}

			if (poolData.expandable)
			{
				return Expand();
			}

			Debug.LogError($"[Pool] {poolData.poolId} is empty and non expandable, can't extract an object.");

			return null;
		}

		internal void BackToPool(GameObject gameObject)
		{
			//check that it's an object from this pool
			Debug.Assert(gameObject != null && gameObject.GetComponent<PoolMember>() != null && pooledObjects.Contains(gameObject)
				, $"[Pool] trying to release object {gameObject?.name} that is not part of pool {poolData.poolId}");

			gameObject.transform.SetParent(poolParent.transform, false);

			gameObject.SetActive(false);

			gameObject.GetComponent<PoolMember>().Available = true;
		}

		internal IEnumerator CreatePoolObjects(Transform parent)
		{
			poolParent = new GameObject(poolData.poolId);
			poolParent.transform.parent = parent;
			for (int i = 0; i < poolData.amount; i++)
			{
				Expand();

				if (i % 10 == 0)// make some pauses :)
				{
					yield return null;
				}
			}
		}

		private GameObject Expand()
		{
			GameObject obj = UnityEngine.Object.Instantiate(poolData.prefab, poolParent.transform);
			obj.SetActive(false); // disable by default
			obj.AddComponent<PoolMember>().ParentPool = this; //add a tracker

			pooledObjects.Add(obj);
			return obj;
		}

		internal void DestroyPoolObjects()
		{
			foreach (GameObject go in pooledObjects)
			{
				go.GetComponent<PoolMember>().Available = false;
				go.GetComponent<PoolMember>().DestroyedByPool = true;
				GameObject.Destroy(go);
			}
			pooledObjects.Clear();
			GameObject.Destroy(poolParent);
		}

		internal void Reset()
		{
			foreach (GameObject go in pooledObjects)
			{
				BackToPool(go);
			}
		}
	}
}