using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgToolkit.AgToolkit.Core.Singleton;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.DataSystem
{
    public class DataSystem
    {
        private static Dictionary<string, AssetBundle> _BundleLoaded = new Dictionary<string, AssetBundle>();

        #region Asset Bundle
        /// <summary>
        /// Load local assetbundle synchronous
        /// </summary>
        public static List<T> LoadLocalBundleSync<T>(string bundleName) where T : Object
        {
            AssetBundle localAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
            
            Debug.Assert(localAssetBundle != null, $"There is no AssetBundle with {bundleName} name.");
            if (localAssetBundle == null)
            {
                return new List<T>();
            }

            List<T> data = localAssetBundle.LoadAllAssets<T>().ToList();
            _BundleLoaded.Add(bundleName, localAssetBundle);

            return data;
        }

        /// <summary>
        /// Load local assetbundle async and return data in the callback
        /// </summary>
        public static IEnumerator LoadLocalBundleAsync<T>(string bundleName, System.Action<List<T>> callback) where T : Object
        {
            AssetBundleCreateRequest asyncRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, bundleName));

            yield return asyncRequest;

            AssetBundle localAssetBundle = asyncRequest.assetBundle;

            Debug.Assert(localAssetBundle != null, $"There is no AssetBundle with {bundleName} name.");
            if (localAssetBundle == null)
            {
                callback(new List<T>());
                yield break;
            }

            AssetBundleRequest assetRequest = localAssetBundle.LoadAllAssetsAsync<T>();
            yield return assetRequest;

            _BundleLoaded.Add(bundleName, localAssetBundle);
            callback(assetRequest.allAssets.Cast<T>().ToList() ); // return data in the callback
        }

        /// <summary>
        /// Load assetBundle from web url and return data in the callback 
        /// </summary>
        public static IEnumerator LoadBundleFromWeb<T>(string url, System.Action<List<T>> callback) where T : Object
        {
            using (WWW web = new WWW(url))
            {
                yield return web;
                AssetBundle remoAssetBundle = web.assetBundle;

                Debug.Assert(remoAssetBundle != null, $"There is no AssetBundle at {url}");
                if (remoAssetBundle == null)
                {
                    callback(new List<T>());
                    yield break;
                }


                _BundleLoaded.Add(url, remoAssetBundle);
                callback(remoAssetBundle.LoadAllAssets<T>().ToList());
            }
        }

        /// <summary>
        /// Unload all AssetBundle
        /// </summary>
        /// <param name="destroyGameObject">Destroy all reference of the asset</param>
        public static void UnloadAllAssetBundles(bool destroyGameObject = false) 
        {
            foreach (AssetBundle ab in _BundleLoaded.Values) 
            {
                ab.Unload(destroyGameObject);
            }

            _BundleLoaded.Clear();
        }

        /// <summary>
        /// Unload the AssetBundle
        /// </summary>
        /// <param name="name">Name or Url of the AssetBundle</param>
        /// <param name="destroyGameObject">Destroy all reference of the asset</param>
        public static void UnloadAssetBundle(string bundle, bool destroyGameObject = false)
        {
            if (!_BundleLoaded.ContainsKey(bundle)) return;

            _BundleLoaded[bundle].Unload(destroyGameObject);
            _BundleLoaded.Remove(bundle);
        }

        #endregion
    }
}
