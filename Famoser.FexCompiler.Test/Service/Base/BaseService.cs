using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test.Service.Base
{
    public abstract class BaseService
    {
        [TestMethod]
        public void TestSimpleFex()
        {
            TestFex("simple.fex");
        }

        [TestMethod]
        public void TestAdvancedFex()
        {
            TestFex("advanced.fex");
        }

        [TestMethod]
        public void TestLongFex()
        {
            TestFex("long.fex");
        }

        protected abstract void TestFex(string fileName);
    }
}
