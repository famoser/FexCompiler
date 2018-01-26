using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Famoser.FexCompiler.Test.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class FexLinesService : BaseService
    {
        protected override void TestFex(string fileName)
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
