using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DebugMenu
{
    public partial class DebugMenuSettingsProvider : Editor
    {
        private const string Name = "Debug Menu";

        public static readonly ISetting EnableKey = new Setting<string>("DEBUG_MENU_SETTINGS_ENABLE_KEY", "Enable Key", "F3");

        [SettingsProvider]
        public static SettingsProvider GetDebugMenuSettings()
        {
            var provider = new SettingsProvider(Name, SettingsScope.User)
            {
                label = Name,
                guiHandler = searchContent =>
                {
                    EnableKey.Set(EditorGUILayout.TextField(EnableKey.DisplayName, (string)EnableKey.Get()));

                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button("Restore Defaults"))
                    {
                        if (EditorUtility.DisplayDialog("Restore Defaults", "Are you sure you want to revert all settings to their default values?", "Yes", "No"))
                        {
                            // Use reflection to get all defined settings and reset them to the default value
                            foreach (ISetting setting in GetDefinedSettings())
                            {
                                setting.Set(setting.DefaultValue);
                            }
                        }
                    }
                },
                keywords = new HashSet<string>(new[] {"Debug Menu"})
            };

            return provider;
        }

        private static IEnumerable<ISetting> GetDefinedSettings()
        {
            foreach (FieldInfo info in typeof(DebugMenuSettingsProvider).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (info.FieldType == typeof(ISetting))
                    yield return (ISetting)info.GetValue(null);
            }
        }
    }
}