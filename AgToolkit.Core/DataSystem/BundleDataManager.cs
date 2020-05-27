using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgToolkit.AgToolkit.Core.DataSystem;
using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.Core.Helper;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AgToolkit.AgToolkit.Core.DataSystem
{
    public class BundleDataManager : Singleton<BundleDataManager>, IBackup
    {

        [SerializeField]
        private List<string> _AssetBundleNames = new List<string>();

        private Dictionary<string, List<Object>> _BundleData = new Dictionary<string, List<Object>>();
        protected override void Awake()
        {
            base.Awake();
            CoroutineManager.Instance.StartCoroutine(Load());
        }
        
        /// <summary>
        /// Get BundleData
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="bundleName">Bundle name</param>
        /// <returns></returns>
        public List<T> GetBundleData<T>(string bundleName)
        {
            if (!_BundleData.ContainsKey(bundleName))
            {
                Debug.LogError($"There is no bundle '{bundleName}' in the bundleData.");
                return new List<T>();
            }

            return _BundleData[bundleName].Cast<T>().ToList();
        }

        public void ClearBundlesLoaded()
        {
            _BundleData.Clear();
            DataSystemManager.UnloadAssetBundles();
        }

        public IEnumerator Save()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Load()
        {
            foreach (string name in _AssetBundleNames)
            {
                yield return DataSystemManager.LoadLocalBundleAsync<Object>(name, list => _BundleData.Add(name, list));
            }
        }
    }
}
