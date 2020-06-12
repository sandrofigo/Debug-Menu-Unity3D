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
                name = $"{MethodInfo.Name} ({parameter})",
                method = MethodInfo,
                monoBehaviour = MethodData.monoBehaviour,
                debugMethod = DebugMethod,
                parameterIndex = index
            });
        }

        public bool HasCustomPath => DebugMethod.customPath != string.Empty;

        public bool HasParameters => DebugMethod.parameters != null;

        public Node GetBaseNode(out bool alreadyExists, IEnumerable<Node> overrideNodeCollection, string overrideName)
        {
            string baseNodeName = overrideName ?? (MethodInfo.DeclaringType == null ? "null" : MethodInfo.DeclaringType.ToString());

            Node baseNode = Helper.GetNodeByName(overrideNodeCollection ?? nodes, baseNodeName);

            alreadyExists = baseNode != null;

            return baseNode ?? new Node
            {
                name = baseNodeName
            };
        }

        public Node GetBaseNode(out bool alreadyExists) => GetBaseNode(out alreadyExists, null, null);

        public Node GetFinalNode() =>
            new Node
            {
                name = MethodInfo.Name,
                method = MethodInfo,
                monoBehaviour = MethodData.monoBehaviour,
                debugMethod = DebugMethod
            };

        public void AttachFinalNodeTo(Node node)
        {
            if (HasParameters)
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
            if (HasCustomPath) // Custom path
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
                Node baseNode = GetBaseNode(out bool alreadyExists);
                if (!alreadyExists)
                    nodes.Add(baseNode);

                AttachFinalNodeTo(baseNode);
            }
        }
    }
}