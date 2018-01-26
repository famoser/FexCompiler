using System.Collections.Generic;
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

            var dic = new Dictionary<string, int>()
            {
                {"simple.fex", 4},
                {"advanced.fex", 11}
            };
            Assert.AreEqual(fexLines.Count, dic[fileName]);

            var dic2 = new Dictionary<string, List<int>>()
            {
                {
                    "simple.fex",
                    new List<int>()
                    {
                        0,1,2,2
                    }
                },
                {"advanced.fex",
                    new List<int>()
                    {
                        0,1,2,3,3,2,2,3,4,4,2
                    }
                }
            };
            for (int i = 0; i < dic2[fileName].Count; i++)
            {
                Assert.AreEqual(fexLines[i].Level, dic2[fileName][i]);
            }
        }

        [TestMethod]
        public void TestSimpleFexLine()
        {
            //arrange
            var fileService = new FileService(TestHelper.GetInputFilePath("simple.fex"));
            var lines = fileService.Process();
            var fexService = new FexService(lines);

            //act
            var fexLines = fexService.Process();

            //assert 
            Assert.IsTrue(fexLines.Count > 0);
        }
    }
}
