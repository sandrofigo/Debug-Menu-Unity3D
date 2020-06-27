using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DebugMenu
{
    public class MethodContext
    {
        public MethodData MethodData { get; }

        public MethodInfo MethodInfo { get; }

        public DebugMethod DebugMethod { get; }

        private readonly List<Node> nodes;

        public MethodContext(MethodData methodData, MethodInfo methodInfo, DebugMethod debugMethod, List<Node> nodes)
        {
            MethodData = methodData;
            MethodInfo = methodInfo;
            DebugMethod = debugMethod;
            this.nodes = nodes;
        }

        public IEnumerable<Node> GetNodesForParameters()
        {
            return DebugMethod.parameters.Select((parameter, index) => new Node
            {
                name = $"{(DebugMethod.HasCustomName ? DebugMethod.customName : MethodInfo.Name)} ({parameter})",
                method = MethodInfo,
                monoBehaviour = MethodData.monoBehaviour,
                debugMethod = DebugMethod,
                parameterIndex = index
            });
        }

        private Node GetBaseNode(out bool alreadyExists, IEnumerable<Node> overrideNodeCollection, string overrideName)
        {
            string baseNodeName = overrideName ?? (MethodInfo.DeclaringType == null ? "null" : MethodInfo.DeclaringType.ToString());

            Node baseNode = Helper.GetNodeByName(overrideNodeCollection ?? nodes, baseNodeName);

            alreadyExists = baseNode != null;

            return baseNode ?? new Node
            {
                name = baseNodeName
            };
        }

        private Node GetFinalNode() =>
            new Node
            {
                name = DebugMethod.HasCustomName ? DebugMethod.customName : MethodInfo.Name,
                method = MethodInfo,
                monoBehaviour = MethodData.monoBehaviour,
                debugMethod = DebugMethod
            };

        private void AttachFinalNodeTo(Node node)
        {
            if (DebugMethod.HasParameters)
            {
                node.children.AddRange(GetNodesForParameters());
            }
            else
            {
                node.children.Add(GetFinalNode());
            }
        }

        public void CreateNodes()
        {
            if (DebugMethod.HasCustomPath) // Custom path
            {
                string[] split = DebugMethod.customPath.Split('/');

                List<Node> currentNodeList = nodes;

                for (int splitIndex = 0; splitIndex < split.Length; splitIndex++)
                {
                    Node baseNode = GetBaseNode(out bool alreadyExists, currentNodeList, split[splitIndex]);

                    if (!alreadyExists)
                        currentNodeList.Add(baseNode);

                    currentNodeList = baseNode.children;

                    if (splitIndex == split.Length - 1)
                    {
                        AttachFinalNodeTo(baseNode);
                    }
                }
            }
            else // Type path
            {
                Node baseNode = GetBaseNode(out bool alreadyExists, null, null);
                if (!alreadyExists)
                    nodes.Add(baseNode);

                AttachFinalNodeTo(baseNode);
            }
        }
    }
}