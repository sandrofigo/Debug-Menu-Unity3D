using System;
using UnityEditor;

namespace DebugMenu
{
    public partial class DebugMenuSettingsProvider
    {
        private class Setting<T> : ISetting
        {
            public string Key { get; }
            public string DisplayName { get; }

            public object DefaultValue { get; }

            public Setting(string key, string displayName, object defaultValue)
            {
                Key = key;
                DisplayName = displayName;
                DefaultValue = defaultValue;
            }

            public void Set(object value)
            {
                switch (value)
                {
                    case string v:
                        EditorPrefs.SetString(Key, v);
                        break;
                    case bool v:
                        EditorPrefs.SetBool(Key, v);
                        break;
                    case int v:
                        EditorPrefs.SetInt(Key, v);
                        break;
                    case float v:
                        EditorPrefs.SetFloat(Key, v);
                        break;
                    default:
                        throw GetTypeNotSupportedException();
                }
            }

            public object Get()
            {
                if (typeof(T) == typeof(string))
                    return EditorPrefs.GetString(Key, (string)DefaultValue);

                if (typeof(T) == typeof(bool))
                    return EditorPrefs.GetBool(Key, (bool)DefaultValue);

                if (typeof(T) == typeof(int))
                    return EditorPrefs.GetInt(Key, (int)DefaultValue);

                if (typeof(T) == typeof(float))
                    return EditorPrefs.GetFloat(Key, (float)DefaultValue);

                throw GetTypeNotSupportedException();
            }

            private static Exception GetTypeNotSupportedException()
            {
                return new Exception($"The provided type {typeof(T).FullName} is not supported!");
            }
        }
    }
}