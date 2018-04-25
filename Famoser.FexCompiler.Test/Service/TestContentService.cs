using System.Collections.Generic;
using Famoser.FexCompiler.Models.Document.Content;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Test.Helpers;
using Famoser.FexCompiler.Test.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test.Service
{
    [TestClass]
    public class TestContentService : BaseService
    {
        protected override void TestFex(string fileName)
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath(fileName));
            var fexService = new FexService(lines);
            var fexLines = fexService.Process();
            var contentService = new ContentService(fexLines);

            //act
            var content = contentService.Process();

            //assert 
            Assert.IsTrue(content.Content.Count > 0);
            Assert.IsTrue(content.TextContent.Count == 0);
            Assert.IsTrue(content.Header == null);

            var dic = new Dictionary<string, int>()
            {
                {"simple.fex", 1},
                {"advanced.fex", 1},
                {"long.fex", 2},
                {"code.fex", 1}
            };
            Assert.AreEqual(content.Content.Count, dic[fileName]);
        }

        [TestMethod]
        public void TestSimpleFexLine()
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath("simple.fex"));
            var fexService = new FexService(lines);
            var fexLines = fexService.Process();
            var contentService = new ContentService(fexLines);

            //act
            var content = contentService.Process();

            //assert 
            Assert.IsTrue(content.Content[0] is Section);
            var section1 = content.Content[0];

            Assert.AreEqual(section1.Header.TextNodes.Count, 1);
            Assert.AreEqual(section1.Header.TextNodes[0].Text, "H1");
            Assert.AreEqual(section1.TextContent.Count, 0);
            Assert.AreEqual(section1.Content.Count, 1);
            Assert.IsTrue(section1.Content[0] is Section);

            var section2 = section1.Content[0];
            Assert.AreEqual(section2.Header.TextNodes.Count, 1);
            Assert.AreEqual(section2.Header.TextNodes[0].Text, "H2");
            Assert.AreEqual(section2.TextContent[0].TextNodes.Count, 1);
            Assert.AreEqual(section2.TextContent[0].TextNodes[0].Text, "hallo");
            Assert.AreEqual(section2.TextContent[1].TextNodes.Count, 1);
            Assert.AreEqual(section2.TextContent[1].TextNodes[0].Text, "welt");
        }

        [TestMethod]
        public void TestCode()
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath("code.fex"));
            var fexService = new FexService(lines);
            var fexLines = fexService.Process();
            var contentService = new ContentService(fexLines);

            //act
            var content = contentService.Process();

            //assert 
            Assert.IsTrue(content.Content[0] != null);
            var section1 = content.Content[0];

            Assert.AreEqual(section1.Header.TextNodes.Count, 1);
            Assert.AreEqual(section1.Header.TextNodes[0].Text, "H1");
            Assert.AreEqual(section1.TextContent.Count, 0);
            Assert.AreEqual(section1.Content.Count, 1);
            Assert.IsTrue(section1.Content[0] != null);

            var section2 = section1.Content[0];
            Assert.AreEqual(section2.Header.TextNodes.Count, 1);
            Assert.AreEqual(section2.Header.TextNodes[0].Text, "H2");
            Assert.AreEqual(section2.TextContent.Count, 4);
            Assert.AreEqual(section2.TextContent[0].TextNodes.Count, 1);
            Assert.AreEqual(section2.TextContent[0].TextNodes[0].Text, "text");
            Assert.AreNotEqual(section2.TextContent[1].CodeNode, null);
            Assert.AreEqual(section2.TextContent[1].CodeNode.Text, "var code = true;\n\tcode = false><;");
            Assert.AreEqual(section2.TextContent[2].TextNodes.Count, 1);
            Assert.AreEqual(section2.TextContent[2].TextNodes[0].Text, "normal text continues");
            Assert.AreEqual(section2.TextContent[3].TextNodes.Count, 1);
            Assert.AreEqual(section2.TextContent[3].TextNodes[0].Text, "and further");
        }

        [TestMethod]
        public void TestLongFexLine()
        {
            //arrange
            var fileService = new FileService();
            var lines = fileService.ReadFile(TestHelper.GetInputFilePath("long.fex"));
            var fexService = new FexService(lines);
            var fexLines = fexService.Process();
            var contentService = new ContentService(fexLines);

            //act
            var content = contentService.Process();

            //assert 
            Assert.IsTrue(content.Content[0] != null);
            Assert.IsTrue(content.Content[1] != null);
            var section1 = content.Content[0];
            var section4 = content.Content[1];

            Assert.AreEqual(section1.Header.TextNodes.Count, 1);
            Assert.AreEqual(section1.Header.TextNodes[0].Text, "H1");
            Assert.AreEqual(section1.TextContent.Count, 0);
            Assert.AreEqual(section1.Content.Count, 2);
            Assert.IsTrue(section1.Content[0] != null);
            Assert.IsTrue(section1.Content[1] != null);

            var section2 = section1.Content[0];
            Assert.AreEqual(section2.Header.TextNodes.Count, 1);
            Assert.AreEqual(section2.Header.TextNodes[0].Text, "H2");
            Assert.AreEqual(section2.TextContent.Count, 0);
            Assert.AreEqual(section2.Content.Count, 1);
            Assert.IsTrue(section2.Content[0] != null);
            
            var section21 = section2.Content[0];
            Assert.AreEqual(section21.Header.TextNodes.Count, 1);
            Assert.AreEqual(section21.Header.TextNodes[0].Text, "H2.1");
            Assert.AreEqual(section21.TextContent.Count, 2); //skip textContent inspection
            Assert.AreEqual(section21.Content.Count, 0);

            var section3 = section1.Content[1];
            Assert.AreEqual(section3.Header.TextNodes.Count, 1);
            Assert.AreEqual(section3.Header.TextNodes[0].Text, "H3");
            Assert.AreEqual(section3.TextContent.Count, 2);
            Assert.AreEqual(section3.Content.Count, 0);
            
            Assert.AreEqual(section4.Header.TextNodes.Count, 1);
            Assert.AreEqual(section4.Header.TextNodes[0].Text, "H4");
            Assert.AreEqual(section4.TextContent.Count, 0);
            Assert.AreEqual(section4.Content.Count, 1);
            Assert.IsTrue(section4.Content[0] != null);

            var section5 = section4.Content[0];
            Assert.AreEqual(section5.Header.TextNodes.Count, 1);
            Assert.AreEqual(section5.Header.TextNodes[0].Text, "H5");
            Assert.AreEqual(section5.TextContent.Count, 2);
            Assert.AreEqual(section5.Content.Count, 0);
        }
    }
}
