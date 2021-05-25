# Core module documentation

The **core module** implements many features that can be used in all types of projects. Here is the list and documentation for all of the features provived.

* [DataSystem](#datasystem)
* [DesignPattern](#designpattern)
  * [Pool](#pool)
  * [Singleton](#singleton)
* [Event](#event)
* [GameMode](#gamemode)
* [Helper](#helper)
* [Manager](#manager)
  * [BundleDataManager](#bundledatamanager) (todo)
  * [CoroutineManager](#coroutinemanager) (todo)
  * [GameManager](#gamemanager) (todo)
  * [SceneLoaderManager](#sceneloadermanager)
  * [TimerManager](#timermanager) (todo)
* [Misc](#misc)
  * [GameEventListeners](#gameeventlisteners) (todo)
  * [GameEvents](#gameevents) (todo)
  * [GameModeSceneContentPair](#gamemodescenecontentpair) (todo)
  * [GameVars](#gamevars) (todo)
  * [SceneContent](#scenecontent)
  * [UnityEvents](#unityevents) (todo)

## DataSystem

This toolkit provide a **DataSystem** to save or load data for your your project.

### Load AssetBundle with DataSystem

When you call the **DataSystem** to load your *AssetBundle* it will always return a `List<T>`. This *List* can be empty if no *AssetBundle* has been found.

**Local *AssetBundle***:
  
  There is 2 ways to load your local *AssetBundle*. 
  
  * Async method:
  
  ```cs 
  public IEnumerator LoadLocalBundleAsync<T>(string bundleName, System.Action<List<T>> callback) where T : Object
  ```
  
*LoadLocalBundleAsync* takes 2 arguments, the name of the bundle you want to load and the callback to call when the data is loaded.
  
  ```cs
  // How to use the LoadLocalBundleAsync method
  [SerializeField]
  string _BundleName = "buildings";
  
  public IEnumerator Load()
  {
    List<Building> _Buildings = null;
    yield return DataSystem.LoadLocalBundleAsync<Building>(_BundleName, data => _Buildings = data);  
  }
  ```
 
  * Sync method: 
  
  ```cs
  public List<T> LoadLocalBundleSync<T>(string bundleName) where T : Object
  ```
  
  *LoadLocalBundleSync* takes 1 argument, the name of the bundle. This method is not recommended if you have a lot of data in your *AssetBundle*.
  
  ```cs
  // How to use the LoadLocalBundleSync method
  [SerializeField]
  string _BundleName = "buildings";
  
  public void Load() 
  {
    List<Building> _Buildings = DataSystem.LoadLocalBundleSync<Building>(_BundleName);
  }
  ```

***AssetBundle* from Web**:

You can load *AssetBundle* from web only in *Async*. 

```cs
public IEnumerator LoadBundleFromWeb<T>(string url, System.Action<List<T>> callback) where T : Object
``` 

*LoadLocalBundleAsync* takes 2 arguments, the URL of the bundle you want to load and the callback to call when the data is loaded.
  
  ```cs
  // How to use the LoadLocalBundleAsync method
  [SerializeField]
  string _BundleName = "buildings";
  
  public IEnumerator Load()
  {
    List<Building> _Buildings = null;
    yield return DataSystem.LoadBundleFromWeb<Building>(_BundleName, data => _Buildings = data);  
  }
  ```

## DesignPattern

### Pool

The object pool pattern is a software creational design pattern that uses a set of initialized objects kept ready to use a "pool" rather than allocating and destroying them on demand. A client of the pool will request an object from the pool and perform operations on the returned object. When the client has finished, it returns the object to the pool rather than destroying it.

This toolkit provide a pool system. You can use it through the singleton **PoolManager**.
As a *Singleton*, it's recommended to attach the **PoolManager** on a gameobject (see [Singleton doc](#singleton)).

There is 2 ways to **create a pool**. 

1. From the Unity editor on the gameobject with the **PoolManager** script attached. (see the screenshot below)
 ![Create Pool Example](/Documentation/Images/AddPoolFromEditor.JPG)
2. From Script : ``yield return PoolManager.Instance.CreatePool(new PoolData("fake", prefab, 50, true, true));``

**PoolData properties**:

1. *poolId*: String -> The identificator of this pool
2. *prefab*: GameObject -> The prefab to instantiate
3. *amount*: Int -> Number of the prefab that will be instantiated 
4. *expandable*: Bool -> If true, the amount will be increase if you try to access to a pool with no pooled object available.
5. *autoSendBack*: Bool -> If true, the pooled object will be send back to his pool when the gameobject will be desactivated. (True is recommended)

**How to use a pooled object**:

```cs
  /// get a pooled object
  GameObject pooledObj = PoolManager.Instance.GetPooledObject("fake");
  // config all data of the object...
  pooledObj.transform.SetParent(yourchoice); //Don't forget to move the gameobject outside of his pool
  pooledObj.SetActive(true); 
 ```
 
 ```cs
  /// deactive a pooled object
  // reset all data of the object...
  pooledObj.SetActive(false);
  pooledObj.SetParent(ThePool); // do that only if the *autoSendBack* param is false
```

### Singleton

In software engineering, the singleton pattern is a software design pattern that restricts the instantiation of a class to one "single" instance. This is useful when exactly one object is needed to coordinate actions across the system.

This toolkit provides an abstract class called **Singleton**. If you need a Singleton in your project you must create your class and inherit from the class provided by this toolkit.

You **must attach your singleton on a gameobject** in a rootscene, you can just use the singleton without attach it on gameobject but **it's not recommended**.

To **access** your Singleton you just have to use the following line : ``` TaskManager.Instance.YourMethod() ```
If the Singleton wasnt instancied at this moment it will be instantiated. 

If you need to check if the Singleton is already instancied you can use the following line : ``` TaskManager.IsInstanced ```

```cs
public class TaskManager : Singleton<TaskManager> 
{ 
  // class code
}
```

If you want to override the **Awake** method, don't forget to call the base method at the begining.

```cs
public class TaskManager : Singleton<TaskManager> 
{ 
   protected override void Awake() 
   {
     base.Awake();
     
     // your code
   }

}
```

If you want to override the **OnApplicationQuit** or **OnDestroy** method, don't forget to call the base method at the end.

```cs
public class TaskManager : Singleton<TaskManager> 
{ 
    protected override void OnApplicationQuit()
    {
      // your code
      base.OnApplicationQuit();
    }

    protected override void OnDestroy()
    {
      // your code
      base.OnDestroy();
    }
}
```

## GameMode

todo


## Helper

### AssetBundle Helper

This toolkit provides an easy way to configure and build an **AssetBundle**.

**Setup data for asset bundle**

1. Create all your data in a folder (eg: All your scriptable object *building*).

![Data For AssetBundle Example](/Documentation/Images/DataInFolder.JPG)

2. Right click on the folder.
3. Create -> AgToolkit -> AssetBundle -> CreateBundleNameRecursively (See below).

 ![Create AssetBundle Name Example](/Documentation/Images/CreateBundleName.jpg)

All asset in the directory (and subdirectories) will be added to a new (or existing) assetbundle with the *directory name* (in lower case). If you don't want to include files from subdirectories, use *CreateBundleName* instead of *CreateBundleNameRecursively*.

 ![Asset With AssetBundle Example](/Documentation/Images/AssetBundleNameSet.JPG)

**Build all AssetBundles**

 ![Build All AssetBundle Example](/Documentation/Images/BuildAssetBundle.jpg)

AssetBundle will be integrated into *Assets/StreamingAssets/*. If the path does not exist, the Toolkit will create it.

To load your AssetBundle data see the [DataSystem](#datasystem) documentation.

### GameMode Helper

todo

### Serialization

With this toolkit, you can serialize a dictionary to show it in the unity inspector window. You have to create a new **SerializableDictionary** and a new **SerializableDictionaryPropertyDrawer**. Then, juste implement your **SerializableDictionary** as a **[SerializeField]** property.

**Create your SerializableDictionary & SerializableDictionaryPropertyDrawer**

```cs
using System;
using AgToolkit.Core.Helper.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AgToolkit.Core.Helper.Drawer;
#endif

[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject>
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StringGameObjectDictionary))]
public class StringGameObjectDictionaryDrawer : SerializableDictionaryPropertyDrawer
{
}
#endif
```

Note the **preprocessor directive** on the **SerializableDictionaryPropertyDrawer** & the `using AgToolkit.Core.Helper.Drawer;`

**Implement your **SerializableDictionary**  as a property**

```cs
[SerializeField]
private StringGameObjectDictionary _prefabToInstantiate= new StringGameObjectDictionary();
``` 

## Manager

### SceneLoaderManager

This toolkit provide a **SceneLoaderManager** to load one or more *Scene* with a *loading scene* and a *lightning scene* in your project. This **manager** can have multiple *persistent scene* and a default *Loading Scene*. You can use it through the singleton **SceneLoaderManager**.
As a *Singleton*, it's recommended to attach the **SceneLoaderManager** on a gameobject (see [Singleton doc](#singleton)).

![SceneLoaderManager Properties](/Documentation/Images/SceneLoaderManager.jpg)

* DefaultLoadingScene: Default loading scene when a *SceneContent* is loaded. if the current *SceneContent* has a *LoadingScene*, the *DefaultLoadingScene* will not be used (check [SceneContent](#scenecontent) doc).
* AdditionalPersistentScenes: Additional scenes that will be persistent. It can be empty.
* MinLoadTimeSec: Represents the minimum loading time. If the current loading time is less than this, the *SceneLoaderManager* will wait until the time is greater than or equal to it. 

Also, you can use this manager from your code to subscribe event and load/Add a persistent scene.

**Events available**
```cs
public event Func<IEnumerator> OnBeforeUnload = null;
public event Func<IEnumerator> OnAfterLoad = null;
public event Func<IEnumerator> OnFadeIn = null;
public event Func<IEnumerator> OnFadeOut = null;
```

**Add a persistent scene**
```cs
SceneLoaderManager.Instance.AddPersistentSceneToLoad(sceneContent);
``` 

**Load a scene**
```cs
SceneLoaderManager.Instance.Load(sceneContent);
``` 

## Misc

### SceneContent

This toolkit provide a **SceneContent** which is a *ScriptableObject*.

To create *SceneContent* follow the steps below:

1. Right click in your **Project window**.
2. Select **SceneContent** in the AgToolkit tab.

![Create SceneContent](/Documentation/Images/CreateSceneContent.jpg)

**SceneContent properties**

![SceneContent Properties](/Documentation/Images/SceneContentProperties.jpg)

* LoadingScene: This is the scene that will be displayed during the loading of *ContentScenes*. It can be null but if it's not null, it will override the *DefaultLoadingScene* of the **SceneLoaderManager**.
* ContentScenes: An array of scene, a scene is required minimum.
* LightningScene: This is the scene with the lightning system, it can be null.
