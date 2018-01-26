using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class FexLinesService
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

        private void TestFex(string fileName)
        {
            //arrange
            var fileService = new FileService(TestHelper.GetInputFilePath(fileName));
            var lines = fileService.Process();
            var fexService = new FexService(lines);

            //act
            var fexLines = fexService.Process();

            //assert 
            Assert.IsTrue(fexLines.Count > 0);
        }
    }
}
