using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DebugMenu
{
    internal static class Helper
    {
        public static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>
        {
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(object), "object"},
            {typeof(bool), "bool"},
            {typeof(char), "char"},
            {typeof(string), "string"},
            {typeof(void), "void"},
            {typeof(byte?), "byte?"},
            {typeof(sbyte?), "sbyte?"},
            {typeof(short?), "short?"},
            {typeof(ushort?), "ushort?"},
            {typeof(int?), "int?"},
            {typeof(uint?), "uint?"},
            {typeof(long?), "long?"},
            {typeof(ulong?), "ulong?"},
            {typeof(float?), "float?"},
            {typeof(double?), "double?"},
            {typeof(decimal?), "decimal?"},
            {typeof(bool?), "bool?"},
            {typeof(char?), "char?"}
        };

        /// <summary>
        /// Returns a <see cref="DebugMethod"/>, if there is no attribute assigned it will return null.
        /// </summary>
        public static DebugMethod GetDebugMethod(MethodInfo method)
        {
            return Attribute.GetCustomAttribute(method, typeof(DebugMethod)) as DebugMethod;
        }

        /// <summary>
        /// Returns a list of <see cref="MethodData"/> from active <see cref="MonoBehaviour"/>s in the scene.
        /// </summary>
        public static IEnumerable<MethodData> GetMethodData()
        {
            var activeMonoBehaviours = Object.FindObjectsOfType<MonoBehaviour>();

            var methods = new List<MethodData>();

            var usedTypes = new HashSet<Type>();

            foreach (MonoBehaviour monoBehaviour in activeMonoBehaviours)
            {
                var methodData = new MethodData();
                
                methodData.methods.AddRange(monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                methodData.monoBehaviour = monoBehaviour;

                if (methodData.methods.Count > 0 && usedTypes.Add(monoBehaviour.GetType()))
                {
                    methods.Add(methodData);
                }
            }

            return methods.ToArray();
        }

        public static Node GetNodeByName(IEnumerable<Node> nodeCollection, string nodeName) => nodeCollection.FirstOrDefault(node => node.name == nodeName);
    }
}