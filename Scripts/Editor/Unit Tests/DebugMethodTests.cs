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
    }
}