using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DebugMenu
{
    public class DebugMenuSettingsProvider : Editor
    {
        private const string Name = "Debug Menu";

        public static readonly ISetting EnableKey = new Setting("DEBUG_MENU_SETTINGS_ENABLE_KEY", "Enable Key", "F3");
        public static readonly ISetting BackgroundColor = new Setting("DEBUG_MENU_SETTINGS_BACKGROUND_COLOR", "Background Color", Color.gray);
        public static readonly ISetting TextColor = new Setting("DEBUG_MENU_SETTINGS_TEXT_COLOR", "Text Color", Color.white);
        
        [SettingsProvider]
        public static SettingsProvider GetDebugMenuSettings()
        {
            var provider = new SettingsProvider(Name, SettingsScope.User)
            {
                label = Name,
                guiHandler = searchContent =>
                {
                    EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
                    
                    EnableKey.Set(EditorGUILayout.TextField(EnableKey.DisplayName, (string)EnableKey.Get()));
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);

                    BackgroundColor.Set(EditorGUILayout.ColorField(BackgroundColor.DisplayName, (Color)BackgroundColor.Get()));
                    TextColor.Set(EditorGUILayout.ColorField(TextColor.DisplayName, (Color)TextColor.Get()));

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
                            
                            EditorGUI.FocusTextInControl(string.Empty);
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