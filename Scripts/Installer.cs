using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DebugMenu
{
    public class Installer : MonoBehaviour
    {
        private const string EnableFileName = ".enable_debug_menu";

        private static bool enableFileExists;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnBeforeSplashScreenRuntimeMethod()
        {
            enableFileExists = File.Exists(EnableFileName);
        }

        private static bool ShouldInstall => Application.isEditor || enableFileExists;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            if (!ShouldInstall)
                return;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Additive)
                return;

            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystemObject = new GameObject {name = "EventSystem"};
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }

            foreach (var item in FindObjectsOfType<DebugMenuManager>())
            {
                DestroyImmediate(item.gameObject);
            }

            GameObject canvas = Instantiate((GameObject)Resources.Load("Prefabs/Debug Menu Canvas", typeof(GameObject)));
            canvas.transform.SetSiblingIndex(0);
            canvas.name = "Debug Menu Canvas";
        }
    }
}