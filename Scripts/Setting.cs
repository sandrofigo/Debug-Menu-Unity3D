using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace DebugMenu
{
    internal class Setting : ISetting
    {
        public string Key { get; }
        public string DisplayName { get; }

        public object DefaultValue { get; }

        /// <remarks>The type that is used to store the data is inferred from <paramref name="defaultValue"/>.</remarks>
        public Setting(string key, string displayName, object defaultValue)
        {
            Key = key;
            DisplayName = displayName;
            DefaultValue = defaultValue;
        }

        public void Set(object value)
        {
#if UNITY_EDITOR
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
                case Color v:
                    EditorPrefs.SetString(Key, v.Serialize());
                    break;
                default:
                    throw new Exception($"The provided type {value.GetType().FullName} is not supported!");
            }
#endif
        }

        public object Get()
        {
#if UNITY_EDITOR
            switch (DefaultValue)
            {
                case string s:
                    return EditorPrefs.GetString(Key, s);
                case bool b:
                    return EditorPrefs.GetBool(Key, b);
                case int i:
                    return EditorPrefs.GetInt(Key, i);
                case float f:
                    return EditorPrefs.GetFloat(Key, f);
                case Color color:
                    return ColorExtension.Deserialize(EditorPrefs.GetString(Key, color.Serialize()));
                default:
                    throw new Exception($"The provided type {DefaultValue.GetType().FullName} is not supported!");
            }
#else
            return DefaultValue;
#endif
        }
    }
}