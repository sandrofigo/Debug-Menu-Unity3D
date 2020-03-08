//
// Copyright (c) Sandro Figo
//
using System;

namespace DebugMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DebugMethod : Attribute
    {
        public DebugMethod()
        {
        }

        public DebugMethod(string customPath)
        {
            this.customPath = customPath;
        }

        public string customPath = string.Empty;
    }
}