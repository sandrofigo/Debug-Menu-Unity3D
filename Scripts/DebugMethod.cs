//
// Copyright (c) Sandro Figo
//
using System;

namespace DebugMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DebugMethod : Attribute
    {
        public DebugMethod(string customPath = "", params object[] parameters)
        {
            if (!string.IsNullOrWhiteSpace(customPath))
                this.customPath = customPath;
            
            this.parameters = parameters.Length == 0 ? null : parameters;
        }

        public DebugMethod(params object[] parameters) : this(string.Empty, parameters)
        {
        }

        public readonly string customPath = string.Empty;

        public readonly object[] parameters;
    }
}