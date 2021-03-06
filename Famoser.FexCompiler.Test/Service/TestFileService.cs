﻿using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Famoser.FexCompiler.Test.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test.Service
{
    [TestClass]
    public class TestFileService : BaseService
    {
        protected override void TestFex(string fileName)
        {
            //arrange
            var fileService = new FileService();

            //act
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath(fileName));

            //assert 
            Assert.IsTrue(lines != null);
            Assert.IsTrue(lines.Length > 0);
        }
    }
}
