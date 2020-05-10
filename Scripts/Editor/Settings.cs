using System.Collections.Generic;
using UnityEditor;

namespace DebugMenu
{
    public class Settings : Editor
    {
        public static string EnableKey
        {
            get => EditorPrefs.GetString("DEBUG_MENU_ENABLE_KEY", "F3");
            set => EditorPrefs.SetString("DEBUG_MENU_ENABLE_KEY", value);
        }

        [SettingsProvider]
        public static SettingsProvider GetDebugMenuSettings()
        {
            var provider = new SettingsProvider("Editor/Debug Menu", SettingsScope.User)
            {
                label = "Debug Menu",
                guiHandler = searchContent =>
                {
                    EnableKey = EditorGUILayout.TextField("DEBUG_MENU_ENABLE_KEY", EnableKey);
                },
                keywords = new HashSet<string>(new[] {"Debug Menu"})
            };

            return provider;
        }
    }
}