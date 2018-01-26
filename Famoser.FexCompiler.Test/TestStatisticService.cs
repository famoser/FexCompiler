using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class TestStatisticService
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
        public void TestSimpleFexExact()
        {
            //arrange
            var fileService = new FileService(TestHelper.GetInputFilePath("simple.fex"));
            var lines = fileService.Process();
            var fexService = new FexService(lines);
            var fexLines = fexService.Process();
            var statisticService = new StatisticService(fexLines);

            //act
            var statistic = statisticService.Process();

            //assert 
            Assert.IsTrue(statistic.CharacterCount == 2 + 2 + 5 + 4);
            Assert.IsTrue(statistic.WordCount == 4);
            Assert.IsTrue(statistic.LineCount == 4);
        }

        private void TestFex(string fileName)
        {
            //arrange
            var fileService = new FileService(TestHelper.GetInputFilePath(fileName));
            var lines = fileService.Process();
            var fexService = new FexService(lines);
            var fexLines = fexService.Process();
            var statisticService = new StatisticService(fexLines);

            //act
            var statistic = statisticService.Process();

            //assert 
            Assert.IsTrue(statistic.CharacterCount > 0);
            Assert.IsTrue(statistic.WordCount > 0);
            Assert.IsTrue(statistic.LineCount > 0);

            Assert.IsTrue(statistic.CharacterCount > statistic.WordCount);
        }
    }
}
