using System;
using System.IO;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Famoser.FexCompiler.Test.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class TestMetaDataService : BaseService
    {
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

        protected override void TestFex(string fileName)
        {
            //arrange
            var configModel = TestHelper.GetConfigModel();
            var metaDataService = new MetaDataService(configModel, TestHelper.GetInputFilePath(fileName));

            //act
            var metaData = metaDataService.Process();

            //assert
            Assert.AreEqual(metaData.Title, fileName.Substring(0, fileName.Length - 4));
            Assert.AreEqual(metaData.Author, configModel.Author);
            Assert.IsTrue(!string.IsNullOrEmpty(metaData.Hash));
            Assert.IsTrue(metaData.ChangedAt > DateTime.MinValue);
            Assert.IsTrue(metaData.GeneratedAt > DateTime.Today);
        }
    }
}
