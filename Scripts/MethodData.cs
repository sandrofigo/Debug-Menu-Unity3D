//
// Copyright (c) Sandro Figo
//

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DebugMenu
{
    public class MethodData
    {
        public MonoBehaviour monoBehaviour = null;
        public readonly List<MethodInfo> methods = new List<MethodInfo>();
    }
}