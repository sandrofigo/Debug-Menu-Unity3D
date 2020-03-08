//
// Copyright (c) Sandro Figo
//
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DebugMenu
{
    public class Node
    {
        public Node()
        {
        }

        public Node(MonoBehaviour monoBehaviour, MethodInfo method, List<Node> children, string name)
        {
            this.monoBehaviour = monoBehaviour;
            this.method = method;
            this.children = children;
            this.name = name;
        }

        public MonoBehaviour monoBehaviour;

        public MethodInfo method;

        public Node parent;

        public readonly List<Node> children = new List<Node>();

        public string name;
    }
}