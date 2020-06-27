# Debug-Menu-Unity3D
A debug menu to call methods on active MonoBehaviours in Unity

## Installation
There are multiple ways to install this package into your project:
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
