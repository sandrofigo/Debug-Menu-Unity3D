using UnityEngine;

namespace DebugMenu
{
    public abstract class Singleton<T> : MonoBehaviour where T : class
    {
        private static T instance;
        
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
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
