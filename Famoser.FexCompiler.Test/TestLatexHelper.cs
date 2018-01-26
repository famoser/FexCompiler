using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class TestLatexHelper
    {
        [TestMethod]
        public void TestAdvancedFex()
        {
            var fileContent = TestHelper.GetInputFile("advanced.fex");
            var configModel = TestHelper.GetConfigModel();
            var doc = FexService.ParseDocument(fileContent.ToList(), "test", configModel);
            TextHelper.Improve(doc);

            var latex = LatexHelper.CreateLatex(doc);
            Assert.IsTrue(latex != "");
        }
    }
}
