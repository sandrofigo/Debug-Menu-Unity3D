using UnityEngine;

namespace DebugMenu
{
    public static class ColorExtension
    {
        public static string Serialize(this Color color) => $"{color.r};{color.g};{color.b};{color.a}";

        public static Color Deserialize(string serialized)
        {
            var split = serialized.Split(';');
            
            return new Color(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
        }
    }
}