using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugMenu
{
    public abstract class Singleton<T> : MonoBehaviour where T : class
    {
        private static T instance;

        private bool keep;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    Debug.LogError($"The singleton ({typeof(T).Name}) you were trying to access was not part of the scene! Add the component to a game object first and try again.");

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;

                SceneManager.sceneUnloaded += scene =>
                {
                    if (!keep)
                        instance = null;
                };
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected void DontDestroyOnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            keep = true;
        }
    }
}
