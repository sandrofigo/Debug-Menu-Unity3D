using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DebugMenu
{
    [CustomEditor(typeof(DebugMenuManager))]
    public class DebugMenuManagerEditor : Editor
    {
        [MenuItem("Debug/Add Debug Menu")]
        private static void CreateDebugMenuObjects(MenuCommand command)
        {
            UnityEngine.Debug.Log("Debug Menu has been added to the current Scene.");

            if (FindObjectOfType<EventSystem>() == null)
                EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");

            foreach (var item in FindObjectsOfType<DebugMenuManager>())
            {
                DestroyImmediate(item.gameObject);
            }

            GameObject canvas = Instantiate((GameObject)Resources.Load("Prefabs/Debug Menu Canvas", typeof(GameObject)));
            canvas.name = "Debug Menu Canvas";

            Undo.RegisterCreatedObjectUndo(canvas, $"Create {canvas.name}");
        }
    }
}