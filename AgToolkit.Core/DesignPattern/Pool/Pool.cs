using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AgToolkit.Core.DesignPattern.Pool
{

	[Serializable]
	public struct PoolData
	{
		[SerializeField]
		private string _poolId;
		[SerializeField]
		private GameObject _prefab;
		[SerializeField, Tooltip("object available for this pool")]
		private int _amount;
		[SerializeField, Tooltip("gameobjects will be instantiated dynamically if amount his too lower")]
		private bool _expandable;
		[SerializeField, Tooltip("Object is sent back to pool automatically on disable if true")]
		private bool _autoSendBack;

        public PoolData(string identifier, GameObject prefabToPool, int amountToPool, bool expandablePool = false, bool autoSendBackMembers = true)
		{
			_poolId = identifier;
			_prefab = prefabToPool;
			_amount = amountToPool;
			_expandable = expandablePool;
			_autoSendBack = autoSendBackMembers;
		}

        public GameObject Prefab => _prefab;
        public string PoolId => _poolId;
        public int Amount => _amount;
        public bool IsAutoSendBack => _autoSendBack;
        public bool IsExpandable => _expandable;

    }

	[Serializable]
	public class Pool
	{

		[SerializeField]
		internal PoolData _poolData;

		private GameObject _poolParent = null;
		private List<GameObject> _pooledObjects = new List<GameObject>();

		internal GameObject GetOne()
		{
			foreach (GameObject g in _pooledObjects)
            {
                PoolMember member = g.GetComponent<PoolMember>();
                if (!g.activeSelf && member.Available)
                {
                    member.BackToPool = false; //cancel back to pool if disabled same frame
                    member.Available = false;
                    return g;
                }
            }

            // If no member available
			if (_poolData.IsExpandable)
			{
				return Expand();
			}

			Debug.LogWarning($"[{GetType().Name}] {_poolData.PoolId} is empty and non expandable, can't extract an object.");

			return null;
		}

		internal void BackToPool(GameObject gameObject)
		{
			//check that it's an object from this pool
			Debug.Assert(gameObject != null && gameObject.GetComponent<PoolMember>() != null && _pooledObjects.Contains(gameObject)
				, $"[{GetType().Name}] trying to release object {gameObject?.name} that is not part of pool {_poolData.PoolId}");

			gameObject.transform.SetParent(_poolParent.transform, false);

			gameObject.SetActive(false);

			gameObject.GetComponent<PoolMember>().Available = true;
		}

		internal void CreatePoolObjects(Transform parent)
		{
			_poolParent = new GameObject(_poolData.PoolId);
			_poolParent.transform.parent = parent;
			for (int i = 0; i < _poolData.Amount; i++)
			{
				Expand();
            }
		}

		private GameObject Expand()
		{
			GameObject obj = UnityEngine.Object.Instantiate(_poolData.Prefab, _poolParent.transform);
			obj.SetActive(false);
			obj.AddComponent<PoolMember>().ParentPool = this;
            _pooledObjects.Add(obj);

			return obj;
		}

		internal void DestroyPoolObjects()
		{
			foreach (GameObject go in _pooledObjects)
			{
				go.GetComponent<PoolMember>().Available = false;
				go.GetComponent<PoolMember>().DestroyedByPool = true;
				Object.Destroy(go);
			}
			_pooledObjects.Clear();
			Object.Destroy(_poolParent);
		}

		internal void Reset()
		{
			foreach (GameObject go in _pooledObjects)
			{
				BackToPool(go);
			}
		}
	}
}