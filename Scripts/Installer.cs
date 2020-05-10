﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DebugMenu
{
    public class Installer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnAfterSceneLoadRuntimeMethod()
        {
            if (!Debug.isDebugBuild && !Application.isEditor) //TODO: use EditorPrefs
                return;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystemObject = new GameObject();
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