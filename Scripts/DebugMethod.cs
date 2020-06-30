//
// Copyright (c) Sandro Figo
//

using System;

namespace DebugMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DebugMethod : Attribute
    {
        public DebugMethod(string customPath = "", string customName = "", bool useReturnValue = false, params object[] parameters)
        {
            if (!string.IsNullOrWhiteSpace(customPath))
                this.customPath = customPath;
            if (!string.IsNullOrWhiteSpace(customName))
                this.customName = customName;

            this.parameters = parameters.Length == 0 ? null : parameters;

            this.useReturnValue = useReturnValue;
        }

        public DebugMethod(string customPath = "", params object[] parameters) : this(customPath, string.Empty, false, parameters)
        {
        }
        
        public DebugMethod(params object[] parameters) : this(string.Empty, string.Empty, parameters)
        {
        }

        public readonly string customPath = string.Empty;
        public bool HasCustomPath => customPath != string.Empty;
        
        public readonly string customName = string.Empty;
        public bool HasCustomName => customName != string.Empty;

        public readonly object[] parameters;
        public bool HasParameters => parameters != null;
        
        public readonly bool useReturnValue;
    }
}