﻿using System;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Services;
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

            //act
            var lines = fileService.Process();

            //assert 
            Assert.IsTrue(lines != null);
            Assert.IsTrue(lines.Length > 0);
        }
    }
}
