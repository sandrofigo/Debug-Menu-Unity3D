//
// Copyright (c) Sandro Figo
//
using System;

namespace DebugMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DebugMethod : Attribute
    {
        public DebugMethod(string customPath = "", object[] parameters = null)
        {
            if (!string.IsNullOrWhiteSpace(customPath))
                this.customPath = customPath;
            
            this.parameters = parameters;
        }

        public DebugMethod(object[] parameters) : this(string.Empty, parameters)
        {
        }

        public readonly string customPath = string.Empty;

        public readonly object[] parameters;
    }
}