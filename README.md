# Debug Menu
A debug menu to call methods on active MonoBehaviours in Unity

[![openupm](https://img.shields.io/npm/v/com.sandrofigo.debug-menu-unity3d?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.sandrofigo.debug-menu-unity3d/)

## Installation
There are multiple ways to install this package into your project:
- Add it to your project through [OpenUPM](https://openupm.com/packages/com.sandrofigo.debug-menu-unity3d/) (recommended)
- Add the package to the Unity package manager using the HTTPS URL of this repository (recommended)
- Download the whole repository as a .zip and place the contents into a subfolder in your assets folder
- Fork the repository and add it as a submodule in git

## Usage
```csharp
using DebugMenu;
using UnityEngine;

public class Foo : MonoBehaviour
{
    [DebugMethod]
    public void Bar()
    {
        Debug.Log("Hello World!");
    }
}
```

This will create a button inside the debug menu which you can access by pressing <kbd>F3</kbd> while the game is running.
Note, this will only work if the MonoBehaviour is active in a scene when it is loaded.

For a more detailed documentation of attributes, please refer to the [Wiki](https://github.com/sandrofigo/Debug-Menu-Unity3D/wiki/Attribute-Usage).

## Collaboration
Support this project with a ⭐️, report an issue or if you feel adventurous and would like to extend the functionality open a pull request.
