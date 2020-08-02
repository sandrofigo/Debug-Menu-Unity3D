using NUnit.Framework;

namespace DebugMenu.Tests
{
    public class DebugMethodTests
    {
        [Test]
        public void CustomPathPropertyWorks()
        {
            DebugMethod method = A.DebugMethod.WithPath("root/sub1/sub2");

            Assert.IsTrue(method.HasCustomPath);
        }

        [Test]
        public void CustomNamePropertyWorks()
        {
            DebugMethod method = A.DebugMethod.WithPath("root/sub1/sub2").WithName("custom name");

            Assert.IsTrue(method.HasCustomName);
        }

        [Test]
        public void ParameterPropertyWorks()
        {
            DebugMethod method1 = A.DebugMethod.WithPath("root/sub1/sub2").WithParameters(new object[] {0, 1, 2, 3});
            DebugMethod method2 = A.DebugMethod.WithParameters(new object[] {0, 1, 2, 3});

            Assert.IsTrue(method1.HasParameters);
            Assert.IsTrue(method2.HasParameters);
        }

        [Test]
        public void FindNodeByPathWorks()
        {
            Node sub2 = A.Node.WithName("Sub2");
            Node sub1 = A.Node.WithName("Sub1").WithChild(sub2);
            Node root = A.Node.WithName("Root").WithChild(sub1);

            DebugMenuManager.AutomaticCreation = true;
            
            DebugMenuManager.Instance.nodes.Add(root);

            Node result1 = DebugMenuManager.Instance.FindNodeByPath("Root.Sub1.Sub2");
            Assert.IsNotNull(result1);

            Node result2 = DebugMenuManager.Instance.FindNodeByPath("Root.Sub1.Sub321");
            Assert.IsNull(result2);
        }
    }
}