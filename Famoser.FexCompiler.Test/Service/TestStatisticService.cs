using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Famoser.FexCompiler.Test.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test.Service
{
    [TestClass]
    public class TestStatisticService : BaseService
    {
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

        protected override void TestFex(string fileName)
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
