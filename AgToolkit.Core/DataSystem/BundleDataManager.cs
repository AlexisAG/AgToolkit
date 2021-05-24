using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgToolkit.Core.DesignPattern.Singleton;
using AgToolkit.Core.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AgToolkit.AgToolkit.Core.DataSystem
{
    public class BundleDataManager : Singleton<BundleDataManager>, IBackup
    {
        [SerializeField]
        private bool _CleanMemoryAfterOnLoad = true;

        [SerializeField, Tooltip("Can be a name or an URL.")]
        private List<string> _AssetBundles = new List<string>();

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
        /// <param name="bundleName">Name or Url</param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destroyGameObject">Destroy all reference of the asset</param>
        public void ClearAllBundlesLoaded(bool destroyGameObject = false)
        {
            _BundleData.Clear();
            DataSystem.UnloadAllAssetBundles(destroyGameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle">Name or Url</param>
        /// <param name="destroyGameObject">Destroy all reference of the asset</param>
        public void UnloadBundleLoaded(string bundle, bool destroyGameObject = false)
        {
            if (!_BundleData.ContainsKey(bundle)) return;


            DataSystem.UnloadAssetBundle(bundle, destroyGameObject);
            _BundleData.Remove(bundle);
        }

        public IEnumerator Save()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Load()
        {
            foreach (string name in _AssetBundles)
            {
                if (Uri.IsWellFormedUriString(name, UriKind.Absolute))
                {
                    yield return DataSystem.LoadBundleFromWeb<Object>(name, list => _BundleData.Add(name, list));
                }
                else
                {
                    yield return DataSystem.LoadLocalBundleAsync<Object>(name, list => _BundleData.Add(name, list));
                }
            }

            if(_CleanMemoryAfterOnLoad) DataSystem.UnloadAllAssetBundles();
        }
    }
}
