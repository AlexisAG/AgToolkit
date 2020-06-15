using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using AgToolkit.AgToolkit.Core.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AgToolkit.AgToolkit.Core.DataSystem
{
    public class DataSystem
    {
        private static string _BinaryFileExtension = ".save";
        private static Dictionary<string, AssetBundle> _BundleLoaded = new Dictionary<string, AssetBundle>();

        private static string GetIndexFile(string dir)
        {
            if (!Directory.Exists(Application.persistentDataPath + '/' + dir))
            {
                Directory.CreateDirectory(Application.persistentDataPath + '/' + dir);
            }
            return Directory.GetFiles(Application.persistentDataPath + '/' + dir).Length.ToString();
        }

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

        #region Binary System

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="data"></param>
        /// <param name="fileId">If fileId is null, the filename will be the index file in the dir</param>
        /// <param name="replaceExisting">If fileId is null, the filename will be the index file in the dir</param>
        public static void SaveGameInBinary(string dir, DataSerializable data, string fileId = null, bool replaceExisting = true)
        {
            FileMode mode = replaceExisting ? FileMode.Create : FileMode.CreateNew;

            if (fileId == null)
            {
                fileId = GetIndexFile(dir);
            }


            if (!Directory.Exists(Application.persistentDataPath + '/' + dir))
            {
                Directory.CreateDirectory(Application.persistentDataPath + '/' + dir);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + dir + "/" + fileId + _BinaryFileExtension, mode);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        /// <summary>
        /// Load data from all files of a directory
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="dir">Directory name</param>
        public static List<T> LoadAllDataFromBinary<T>(string dir) where T : DataSerializable
        {
            List<T> data = new List<T>();
            BinaryFormatter formatter = new BinaryFormatter();

            if (!Directory.Exists(Application.persistentDataPath + '/' + dir)) return data;

            Directory.GetFiles(Application.persistentDataPath + '/' + dir).ToList().ForEach((s =>
            {
                FileStream stream = new FileStream(s, FileMode.Open);

                data.Add(formatter.Deserialize(stream) as T);

                stream.Close();
            }));

            return data;
        }

        /// <summary>
        /// Load data from a file
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="dir">directory name</param>
        /// <param name="filename">file name</param>
        public static T LoadDataFromBinary<T>(string dir, string filename)  where T : DataSerializable
        {
            string path = Application.persistentDataPath + '/' + dir + "/" + filename + _BinaryFileExtension;

            if (!File.Exists(path)) return null;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Close();

            return formatter.Deserialize(stream) as T;
        }
        #endregion


    }
}
