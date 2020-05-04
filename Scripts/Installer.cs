using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DebugMenu
{
    public class Installer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnAfterSceneLoadRuntimeMethod()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (FindObjectOfType<EventSystem>() == null)
                EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");

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