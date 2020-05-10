namespace DebugMenu
{
    public partial class DebugMenuSettingsProvider
    {
        public interface ISetting
        {
            string Key { get; }
            string DisplayName { get; }
            object DefaultValue { get; }
            void Set(object value);
            object Get();
        }
    }
}