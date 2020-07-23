using System.Collections.Generic;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Services.Latex;
using Famoser.FexCompiler.Test.Helpers;
using Famoser.FexCompiler.Test.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test.Service
{
    [TestClass]
    public class TestLatexService : BaseService
    {
        [TestMethod]
        public void TestFormulas()
        {
            TestFex("formulas.fex");
        }

        protected override void TestFex(string fileName)
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath(fileName));
            var fexService = new FexService(lines);
            var fexLines = fexService.Process();
            var contentService = new ContentService(fexLines);
            var root = contentService.Process();
            var latexService = new GenerationService(new StatisticModel(), new MetaDataModel(), root.Children);
            var dict = new Dictionary<string, string[]>()
            {
                {
                    "simple.fex",
                    new []
                    {
                        @"\textbf{H2}\\",
                        @"hallo\\",
                        @"welt\\"
                    }
                },
                {
                    "advanced.fex",
                    new []
                    {
                        @"\section{H1}",
                        @"\subsection{H2}",
                        @"\subsubsection{text}",
                        @"\textbf{because it has further}"
                    }
                },
                {
                    "long.fex",
                    new []
                    {
                        @"\subsection{H3}",
                        @"H3.1\\",
                        @"\section{H4}"
                    }
                },
                {
                    "formulas.fex",
                    new []
                    {
                        @"$B_{ak}$",
                        @"$x^{-2t}$",
                        @"$n^{-1000}$",
                    }
                },
            };

            //act
            var latex = latexService.Process();

            //assert 
            foreach (var s in dict[fileName])
            {
                Assert.IsTrue(latex.Contains(s));
            }
        }
    }
}
