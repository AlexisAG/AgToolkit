The **core module** implements many features that can be used in all types of projects. Here is the list and documentation for all of the features provived.

* [Singleton](#singleton)
* [Pool](#pool)
* [Loader](#loader)
* [GameMode](#gamemode)
* [Helper](#helper)
* [TimeManager](#timemanager)


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

todo

## Loader

todo


## GameMode

todo


## Helper

todo


## TimeManager

todo
