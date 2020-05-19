# AgToolkit
AgToolkit is a toolkit for Unity 3D. 
It was built to simplify development with the Unity 3D game engine.

## Getting started with AgToolkit

1. Create your github project.
2. Create your Unity Project (Version 2019.X.X).
3. Add the toolkit as a submodule.
   * from a command line: `git submodule add https://github.com/AlexisAG/AgToolkit.git` 
   * from fork ![Fork example](/Documentation/Images/AddSubmoduleFromFork.JPG)
4. Create Symlink in the Asset folder of your Unity Project for each desired module.
   * Command line example for **Core module**: 
     * `mklink /D D:\yourGithubProject\yourUnityProject\Assets\AgToolkit.core D:\yourGithubProject\AgToolkit\AgToolkit.Core`

## Branch

* **Master** is the lastest [Milestone](https://github.com/AlexisAG/AgToolkit/milestones) released.
* **Dev** has all new features and bug fixes but may be unstable. I do **not** recommend to use it without **tag**.

## Documentation

* The **core module** implements many features that can be used in all types of projects. You can find the **documentation** [here](/AgToolkit.Core/core.md)
* The **network module** implements many features for projects requiring a multiplayer mode. Inside this module, you can find different implementations (like Network.Photon) or simply use the network module to create your implementation with another library. For now there is no **documentation**.

## Community and support

If you'd like to discuss your issues or ideas to improve this toolkit, you can **[create an issue](https://github.com/AlexisAG/AgToolkit/issues/new/choose)** on this project.

## Author

[Alexis Gay](https://www.linkedin.com/in/alexis-gay-link/)

## License

MIT
