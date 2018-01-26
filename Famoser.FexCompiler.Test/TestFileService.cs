using System;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class TestFileService
    {
        [TestMethod]
        public void TestSimpleFex()
        {
            var fileContent = TestHelper.GetInputFile("simple.fex");
            var configModel = TestHelper.GetConfigModel();
        }

        [TestMethod]
        public void TestAdvancedFex()
        {
            var fileContent = TestHelper.GetInputFile("advanced.fex");
            var configModel = TestHelper.GetConfigModel();
        }
    }
}
