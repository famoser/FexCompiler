using System;
using System.IO;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class TestMetaDataService
    {
        [TestMethod]
        public void TestSimpleFex()
        {
            TestFex("simple.fex", "simple");
        }

        [TestMethod]
        public void TestAdvancedFex()
        {
            TestFex("advanced.fex", "advanced");
        }

        [TestMethod]
        public void TestHashFex()
        {
            //arrange
            var configModel = TestHelper.GetConfigModel();
            var metaDataService1 = new MetaDataService(configModel, TestHelper.GetInputFilePath("simple.fex"));
            var metaDataService2 = new MetaDataService(configModel, TestHelper.GetInputFilePath("advanced.fex"));

            //act
            var metaData1 = metaDataService1.Process();
            var metaData2 = metaDataService2.Process();

            //assert
            Assert.AreNotEqual(metaData1.Hash, metaData2.Hash);
        }

        private void TestFex(string fileName, string expectedName)
        {
            //arrange
            var configModel = TestHelper.GetConfigModel();
            var metaDataService = new MetaDataService(configModel, TestHelper.GetInputFilePath(fileName));

            //act
            var metaData = metaDataService.Process();

            //assert
            Assert.AreEqual(metaData.Title, expectedName);
            Assert.AreEqual(metaData.Author, configModel.Author);
            Assert.IsTrue(!string.IsNullOrEmpty(metaData.Hash));
            Assert.IsTrue(metaData.ChangedAt > DateTime.MinValue);
            Assert.IsTrue(metaData.GeneratedAt > DateTime.Today);
        }
    }
}
