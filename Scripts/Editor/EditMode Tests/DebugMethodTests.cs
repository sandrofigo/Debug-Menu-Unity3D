using NUnit.Framework;

namespace DebugMenu.Tests
{
    public class DebugMethodTests
    {
        [Test]
        public void CustomPathPropertyWorks()
        {
            var method = new DebugMethod("root/sub1/sub2");

            Assert.IsTrue(method.HasCustomPath);
        }

        [Test]
        public void CustomNamePropertyWorks()
        {
            var method = new DebugMethod("root/sub1/sub2", "custom name");

            Assert.IsTrue(method.HasCustomName);
        }

        [Test]
        public void ParameterPropertyWorks()
        {
            var method1 = new DebugMethod("root/sub1/sub2", 0, 1, 2, 3);
            var method2 = new DebugMethod(0, 1, 2, 3);

            Assert.IsTrue(method1.HasParameters);
            Assert.IsTrue(method2.HasParameters);
        }

        [Test]
        public void FindNodeByPathWorks()
        {
            Node root = new Node
            {
                name = "Root"
            };

            Node sub1 = new Node
            {
                name = "Sub1"
            };

            Node sub2 = new Node
            {
                name = "Sub2"
            };

            root.children.Add(sub1);
            sub1.children.Add(sub2);

            DebugMenuManager.AutomaticCreation = true;

            DebugMenuManager.Instance.nodes.Add(root);

            Node result1 = DebugMenuManager.Instance.FindNodeByPath("Root.Sub1.Sub2");
            Assert.IsNotNull(result1);
            
            Node result2 = DebugMenuManager.Instance.FindNodeByPath("Root.Sub1.Sub321");
            Assert.IsNull(result2);
        }
    }
}