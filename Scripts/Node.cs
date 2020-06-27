//
// Copyright (c) Sandro Figo
//
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DebugMenu
{
    public class Node
    {
        public MonoBehaviour monoBehaviour;

        public MethodInfo method;
        
        public readonly List<Node> children = new List<Node>();

        public string name;

        public DebugMethod debugMethod;

        public int parameterIndex = -1;

        public bool HasChildren() => children.Any();
    }
}