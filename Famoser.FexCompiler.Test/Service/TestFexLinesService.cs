﻿using System.Collections.Generic;
using System.Linq;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Famoser.FexCompiler.Test.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test.Service
{
    [TestClass]
    public class TestFexLinesService : BaseService
    {
        protected override void TestFex(string fileName)
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath(fileName));
            var fexService = new FexService(lines);

            //act
            var fexLines = fexService.Process();

            //assert 
            Assert.IsTrue(fexLines.Count > 0);

            var dic = new Dictionary<string, int>()
            {
                {"simple.fex", 4},
                {"advanced.fex", 11},
                {"long.fex", 12}
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
                {
                    "advanced.fex",
                    new List<int>()
                    {
                        0,1,2,3,3,2,2,3,4,4,2
                    }
                },
                {
                    "long.fex",
                    new List<int>()
                    {
                        0,1,2,3,3,1,2,2,0,1,2,2
                    }
                }
            };
            for (int i = 0; i < dic2[fileName].Count; i++)
            {
                Assert.AreEqual(fexLines[i].Level, dic2[fileName][i]);
            }
        }
        [TestMethod]
        public void TestColon()
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath("colon.fex"));
            var fexService = new FexService(lines);

            //act
            var fexLines = fexService.Process();

            //assert 
            Assert.AreEqual(9, fexLines.Count);

            var levelList = new List<int>() { 0, 1, 1, 2, 2, 1, 2, 1, 1 };
            for (int i = 0; i < fexLines.Count; i++)
            {
                Assert.AreEqual(levelList[i], fexLines[i].Level);
            }

            Assert.AreEqual(0, fexLines.Count(l => l.Text.EndsWith(":")));
        }

        [TestMethod]
        public void TestSimpleFexLine()
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath("simple.fex"));
            var fexService = new FexService(lines);

            //act
            var fexLines = fexService.Process();

            //assert 
            Assert.IsTrue(fexLines.Count > 0);
        }
    }
}
