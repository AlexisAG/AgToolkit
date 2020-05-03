using System.Collections;
using System.Collections.Generic;
using AgToolkit.AgToolkit.Core.Singleton;
using UnityEngine;

namespace AgToolkit.Core.Pool
{
	public class PoolManager : Singleton<PoolManager>
	{
		[SerializeField]
		private List<Pool> _pools = new List<Pool>();

		private GameObject _poolsParent = null;

		private bool _lock = true;

		protected override void Awake()
		{
			base.Awake();
			_poolsParent = gameObject;
		}

		protected override void OnApplicationQuit()
		{
			DestroyAllPools();
			base.OnApplicationQuit();
		}
		protected override void OnDestroy()
		{
			DestroyAllPools();
			base.OnDestroy();
		}

		private IEnumerator Start()
		{
            if (_pools.Count > 0)
            {
                //create objects for already existing pools (set from inspector)
                foreach (Pool p in _pools)
                {
                    p.CreatePoolObjects(_poolsParent.transform);
                    yield return null;
                }

                Debug.Log($"[{this.GetType().Name}] {_pools.Count} Pools created.");
            }
			_lock = false;
		}

        /// <summary>
        /// Create new Pool, use it when your game is loading (OnLoad())
        /// </summary>
        public IEnumerator CreatePool(PoolData poolData)
		{
			while (_lock)
			{
				yield return null;
			}
			_lock = true;

			Debug.Assert(!PoolExists(poolData.PoolId), $"[{this.GetType().Name}] can't create a pool with identifier {poolData.PoolId}, one already exists.");

			//create new pool
			Pool pool = new Pool()
			{
                _poolData = poolData
			};
			pool.CreatePoolObjects(_poolsParent.transform);

			_pools.Add(pool);

			_lock = false;
		}

        /// <summary>
        /// Return PoolData (not the gameobject)
        /// </summary>
        public PoolData GetPoolData(string identifier)
		{
			Pool pool = GetPool(identifier);
			return pool._poolData;
		}

        /// <summary>
        /// Destroy all gameobject of this pool and remove the pool
        /// </summary>
        public void DestroyPool(string identifier)
		{
			Debug.Assert(PoolExists(identifier), $"[{this.GetType().Name}] can't destroy pool no pools with identifier {identifier} exists.");
			Pool pool = GetPool(identifier);

			pool.DestroyPoolObjects();
			_pools.Remove(pool);
		}

        private void DestroyAllPools()
		{
			foreach (Pool p in _pools)
			{
				p.DestroyPoolObjects();
			}
			_pools.Clear();
		}

		/// <summary>
		/// returns an object from pool
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns>an object from the pool (with active = false)</returns>
		public GameObject GetPooledObject(string identifier)
		{
			return GetPool(identifier).GetOne();
		}

		private Pool GetPool(string identifier)
		{
			Debug.Assert(PoolExists(identifier), $"[{this.GetType().Name}] can't get pool, no pools with identifier '{identifier}' exists.");
            return _pools.Find(p => p._poolData.PoolId == identifier);
		}

		public bool PoolExists(string identifier)
		{
			return _pools.Exists(p => p._poolData.PoolId == identifier);
		}
	}

}