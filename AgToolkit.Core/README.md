# Core module documentation

The **core module** implements many features that can be used in all types of projects. Here is the list and documentation for all of the features provived.

* [Singleton](#singleton)
* [Pool](#pool)
* [Loader](#loader)
* [GameMode](#gamemode)
* [Helper](#helper)
* [TimerManager](#timermanager)


## Singleton

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



## Pool

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
  poolObj.transform.SetParent(yourchoice); //Don't forget to move the gameobject outside of his pool
  poolObj.SetActive(true); 
  
  /// deactive a pooled object
  // reset all data of the object...
  poolObj.SetActive(false);
  poolObj.SetParent(ThePool); // do that only if the *autoSendBack* param is false
```

## Loader

This toolkit provide a **Loader** to load one or more *Scene* with a *loading scene* and a *lightning scene* in your project. This **Loader** can have multiple *persistent scene* and a default *Loading Scene*. You can use it through the singleton **SceneLoaderManager**.

As a *Singleton*, it's recommended to attach the **PoolManager** on a gameobject (see [Singleton doc](#singleton)).

This toolkit provide a **SceneContent** which is a *ScriptableObject*. In fact, the *SceneLoaderManager* use a *SceneContent*.

To create *SceneContent* follow the steps below:

1. Right click in your **Project window**.
2. Select **SceneContent** in the AgToolkit tab.

![Create SceneContent](/Documentation/Images/CreateSceneContent.jpg)

**SceneContent properties**

![SceneContent Properties](/Documentation/Images/SceneContentProperties.jpg)

* LoadingScene: This is the scene that will be displayed during the loading of *ContentScenes*. It can be null but if it's not null, it will override the *DefaultLoadingScene* of the **SceneLoaderManager**.
* ContentScenes: An array of scene, a scene is required minimum.
* LightningScene: This is the scene with the lightning system, it can be null.

**SceneLoaderManager properties**:

![SceneLoaderManager Properties](/Documentation/Images/SceneLoaderManager.jpg)

* DefaultLoadingScene: Default loading scene when a *SceneContent* is loaded. if the current *SceneContent* has a *LoadingScene*, the *DefaultLoadingScene* will not be used.
* AdditionalPersistentScenes: Additional scenes that will be persistent. It can be empty.
* MinLoadTimeSec: Represents the minimum loading time. If the current loading time is less than this, the *SceneLoaderManager* will wait until the time is greater than or equal to it. 


## GameMode

todo


## Helper

todo


## TimerManager

todo
