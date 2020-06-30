//
// Copyright (c) Sandro Figo
//

using System;

namespace DebugMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DebugMethod : Attribute
    {
        private DebugMethod(string customPath = "", string customName = "", bool useReturnValue = false, params object[] parameters)
        {
            if (!string.IsNullOrWhiteSpace(customPath))
                Path = customPath;
            if (!string.IsNullOrWhiteSpace(customName))
                Name = customName;

            Parameters = parameters.Length == 0 ? null : parameters;

            UseReturnValue = useReturnValue;
        }

        public DebugMethod(params object[] parameters) : this(string.Empty, string.Empty, false, parameters)
        {
        }

        public string Path { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public object[] Parameters { get; set; }

        public bool UseReturnValue { get; set; }

        public bool HasCustomPath => Path != string.Empty;

        public bool HasCustomName => Name != string.Empty;

        public bool HasParameters => Parameters != null;
    }
}