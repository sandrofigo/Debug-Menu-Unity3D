using UnityEngine;

namespace DebugMenu
{
    public static class Settings
    {
        public static readonly ISetting EnableKeySetting = new Setting("DEBUG_MENU_SETTINGS_ENABLE_KEY", "Enable Key", "F3");

        public static string EnableKey
        {
            get => (string)EnableKeySetting.Get();
            set => EnableKeySetting.Set(value);
        }
        
        public static readonly ISetting BackgroundColorSetting = new Setting("DEBUG_MENU_SETTINGS_BACKGROUND_COLOR", "Background Color", new Color(0.31f, 0.31f, 0.31f));
        
        public static Color BackgroundColor
        {
            get => (Color)BackgroundColorSetting.Get();
            set => BackgroundColorSetting.Set(value);
        }
        
        public static readonly ISetting TextColorSetting = new Setting("DEBUG_MENU_SETTINGS_TEXT_COLOR", "Text Color", Color.white);
        
        public static Color TextColor
        {
            get => (Color)TextColorSetting.Get();
            set => TextColorSetting.Set(value);
        }
        
        public static readonly ISetting HideConsoleSetting = new Setting("DEBUG_MENU_SETTINGS_HIDE_CONSOLE", "Hide Console", false);
        
        public static bool HideConsole
        {
            get => (bool)HideConsoleSetting.Get();
            set => HideConsoleSetting.Set(value);
        }
        
        public static readonly ISetting AutoClosePanelsSetting = new Setting("DEBUG_MENU_SETTINGS_AUTO_CLOSE_PANELS", "Auto Close Panels", true);
        
        public static bool AutoClosePanels
        {
            get => (bool)AutoClosePanelsSetting.Get();
            set => AutoClosePanelsSetting.Set(value);
        }
        
    }
}