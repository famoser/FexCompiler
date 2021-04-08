using System.Collections.Generic;
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

        [TestMethod]
        public void TestRealWorld()
        {
            TestFex("realworld.fex");
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
            var latexService = new GenerationService(root.Children);
            var dict = new Dictionary<string, string[]>
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
                        @"\section{H1",
                        @"\subsection{H2",
                        @"\subsubsection{text",
                        @"\textbf{because it has further"
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
                        @"x$^{2_{32}}$", // looks weird, but rendering checks out
                        @"x$^{2_{32}}$$_{bottom}$(a)",
                        @"$x^2$\\",
                        @"$x^{2^{32}}$\\",
                        @"$x^{2_{32}}$\\",
                        @"$K_{{ab}_{ab}}$\\",
                        @"$x^{-2t}$\\",
                        @"$n^{-1000}$\\",
                        @"$\epsilon$\\",
                        @"hello $\theta$ how are you\\",
                    }
                },
                {
                    "realworld.fex",
                    new []
                    {
                        @"some condition \textasciicircum",
                        @"${\{N\}}_{\{A\}}$\\",
                        @"S $\rightarrow$ B: M, ${\{N_A, K_{AB}\}}_{AS}$, ${\{N_B, K_{AB}\}}_{BS}$\\",
                        @"${\{N\}}_1$\\",
                        @"${\{N_A\}}_1$\\",
                        @"B $\rightarrow$ A: $g^y$, ${\{g^y, g^x, A\}}_{sk(B)}$\\",
                        @"${\{A, B\}}_{sk}$\\",
                        @"${\{A, B\}}_{sk(B)}$\\",
                        @"${p(2)\{2\}}_{20p(8)(9)asd}$ hello\\"
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
