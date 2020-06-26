using UnityEngine;

namespace DebugMenu
{
    public interface ISetting
    {
        string Key { get; }
        string DisplayName { get; }
        string Tooltip { get; }
        object DefaultValue { get; }
        void Set(object value);
        object Get();
        GUIContent GetGuiContent();
    }
}