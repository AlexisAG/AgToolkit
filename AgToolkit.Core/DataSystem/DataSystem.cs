﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgToolkit.AgToolkit.Core.Singleton;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.DataSystem
{
    public class DataSystem : Singleton<DataSystem>
    {
        /// <summary>
        /// Load local assetbundle synchronous
        /// </summary>
        public List<T> LoadLocalBundleSync<T>(string bundleName) where T : Object
        {
            AssetBundle localAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
            
            Debug.Assert(localAssetBundle != null, $"There is no AssetBundle with {bundleName} name.");
            if (localAssetBundle == null) return null;

            List<T> data = localAssetBundle.LoadAllAssets<T>().ToList();
            localAssetBundle.Unload(false);

            return data;
        }

        /// <summary>
        /// Load local assetbundle async and return data in the callback
        /// </summary>
        public IEnumerator LoadLocalBundleAsync<T>(string bundleName, System.Action<List<T>> callback) where T : Object
        {
            AssetBundleCreateRequest asyncRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, bundleName));

            yield return asyncRequest;

            AssetBundle localAssetBundle = asyncRequest.assetBundle;

            Debug.Assert(localAssetBundle != null, $"There is no AssetBundle with {bundleName} name.");
            if (localAssetBundle == null) yield break;

            AssetBundleRequest assetRequest = localAssetBundle.LoadAllAssetsAsync<T>();
            yield return assetRequest;

            callback(assetRequest.allAssets.Cast<T>().ToList() ); // return data in the callback
        }
    }
}