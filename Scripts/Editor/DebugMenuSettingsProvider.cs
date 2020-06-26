﻿using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DebugMenu
{
    public class DebugMenuSettingsProvider : Editor
    {
        private const string Name = "Debug Menu";

        [SettingsProvider]
        public static SettingsProvider GetDebugMenuSettings()
        {
            var provider = new SettingsProvider(Name, SettingsScope.User)
            {
                label = Name,
                guiHandler = searchContent =>
                {
                    EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
                    
                    Settings.EnableKey = EditorGUILayout.TextField(Settings.EnableKeySetting.DisplayName, Settings.EnableKey);
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);

                    Settings.BackgroundColor = EditorGUILayout.ColorField(Settings.BackgroundColorSetting.DisplayName, Settings.BackgroundColor);
                    Settings.TextColor = EditorGUILayout.ColorField(Settings.TextColorSetting.DisplayName, Settings.TextColor);

                    EditorGUILayout.Space();
                    
                    Settings.HideConsole = EditorGUILayout.Toggle(Settings.HideConsoleSetting.DisplayName, Settings.HideConsole);
                    
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
            foreach (FieldInfo info in typeof(Settings).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (info.FieldType == typeof(ISetting))
                    yield return (ISetting)info.GetValue(null);
            }
        }
    }
}