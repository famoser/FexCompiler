using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected abstract void TestFex(string fileName);
    }
}
