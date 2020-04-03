using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgToolkit.AgToolkit.Core.Singleton;
using FramaToolkit;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Pool
{
	public class ObjectPooler : Singleton<ObjectPooler>
	{
		[SerializeField]
		private List<Pool> Pools = null;

		private GameObject PoolsParent = null;

		private bool _lock = true;

		protected override void Awake()
		{
			base.Awake();
			PoolsParent = gameObject;//fallback to this gameobject
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
			if (Pools == null)//singleton created on runtime
			{
				Pools = new List<Pool>();
				yield break;
			}
			else
			{
				//create objects for already existing pools (set from inspector)
				foreach (Pool p in Pools)
				{
					yield return p.CreatePoolObjects(PoolsParent.transform);
				}
				Debug.Log($"[ObjectPooler] {Pools.Count} Pools created.");
			}
			_lock = false;
		}

		public IEnumerator CreatePool(string identifier, GameObject prefabToPool, int amountToPool, bool expandable = false, bool autoSendBack = true)
		{
			yield return CreatePool(new PoolData(identifier, prefabToPool, amountToPool, expandable, autoSendBack));

		}
		public IEnumerator CreatePool(PoolData poolData)
		{
			while (_lock)//wait for creation
			{
				yield return null;
			}
			_lock = true;

			Debug.Assert(!PoolExists(poolData.poolId), $"[ObjectPooler] can't create a pool with identifier {poolData.poolId}, one already exists.");

			//create new pool
			Pool pool = new Pool()
			{
				poolData = poolData
			};
			yield return pool.CreatePoolObjects(PoolsParent.transform);

			Pools.Add(pool);

			_lock = false;
		}

		public PoolData GetPoolData(string identifier)
		{
			Pool pool = GetPool(identifier);
			return pool.poolData;
		}

		public void DestroyPool(string identifier)
		{
			Debug.Assert(PoolExists(identifier), $"[ObjectPooler] can't destroy pool no pools with identifier {identifier} exists.");
			Pool pool = GetPool(identifier);

			pool.DestroyPoolObjects();
			Pools.Remove(pool);
		}

		private void DestroyAllPools()
		{
			foreach (Pool p in Pools)
			{
				p.DestroyPoolObjects();
			}
			Pools.Clear();
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
			Debug.Assert(PoolExists(identifier), $"[ObjectPooler] can't get pool, no pools with identifier '{identifier}' exists.");
			return Pools.FirstOrDefault(p => p.poolData.poolId == identifier);
		}

		public bool PoolExists(string identifier)
		{
			return Pools.Any(p => p.poolData.poolId == identifier);
		}
	}

}