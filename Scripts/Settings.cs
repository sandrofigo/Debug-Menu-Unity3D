using UnityEngine;

namespace DebugMenu
{
    public static class Settings
    {
        public static readonly ISetting EnableKey = new Setting("DEBUG_MENU_SETTINGS_ENABLE_KEY", "Enable Key", "F3");
        public static readonly ISetting BackgroundColor = new Setting("DEBUG_MENU_SETTINGS_BACKGROUND_COLOR", "Background Color", new Color(0.31f, 0.31f, 0.31f));
        public static readonly ISetting TextColor = new Setting("DEBUG_MENU_SETTINGS_TEXT_COLOR", "Text Color", Color.white);
    }
}