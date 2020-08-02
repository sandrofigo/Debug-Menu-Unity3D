using System.Collections.Generic;

namespace DebugMenu.Tests
{
    public class NodeBuilder : Builder<Node>
    {
        private string _name;

        private readonly List<Node> _children = new List<Node>();

        public NodeBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public NodeBuilder WithChildren(IEnumerable<Node> children)
        {
            _children.AddRange(children);
            return this;
        }

        public NodeBuilder WithChild(Node child)
        {
            _children.Add(child);
            return this;
        }

        public override Node Build()
        {
            var node = new Node
            {
                name = _name
            };
            
            node.children.AddRange(_children);

            return node;
        }
    }
}