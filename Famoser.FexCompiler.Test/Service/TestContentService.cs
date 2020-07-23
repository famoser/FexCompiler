using System.Collections.Generic;
using Famoser.FexCompiler.Models.Document;
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
            Assert.IsTrue(content.Children.Count > 0);
            Assert.IsTrue(content.Content.Count == 0);
            Assert.IsTrue(content.Header == null);

            var dic = new Dictionary<string, int>()
            {
                {"simple.fex", 1},
                {"advanced.fex", 1},
                {"long.fex", 2},
                {"codeContent.fex", 1}
            };
            Assert.AreEqual(content.Children.Count, dic[fileName]);
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
            Assert.IsTrue(content.Children[0] is Section);
            var section1 = content.Children[0];

            Assert.AreEqual(section1.Header.Text, "H1");
            Assert.AreEqual(section1.Content.Count, 0);
            Assert.AreEqual(section1.Children.Count, 1);
            Assert.IsTrue(section1.Children[0] is Section);

            var section2 = section1.Children[0];
            Assert.AreEqual(section2.Header.Text, "H2");
            Assert.AreEqual(section2.Content[0].Text, "hallo");
            Assert.AreEqual(section2.Content[1].Text, "welt");
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
            Assert.IsTrue(content.Children[0] != null);
            var section1 = content.Children[0];

            Assert.AreEqual(section1.Header.ContentType, ContentType.Text);
            Assert.AreEqual(section1.Header.Text, "H1");
            Assert.AreEqual(section1.Content.Count, 0);
            Assert.AreEqual(section1.Children.Count, 1);
            Assert.IsTrue(section1.Children[0] != null);

            var section2 = section1.Children[0];
            Assert.AreEqual(section2.Header.ContentType, ContentType.Text);
            Assert.AreEqual(section2.Header.Text, "H2");
            Assert.AreEqual(section2.Content.Count, 4);
            
            Assert.AreEqual(section2.Content[0].ContentType, ContentType.Text);
            Assert.AreEqual(section2.Content[0].Text, "text");

            Assert.AreEqual(section2.Content[1].ContentType, ContentType.Code);
            Assert.AreEqual(section2.Content[1].Text, "var code = true;\n\tcode = false><;");

            Assert.AreEqual(section2.Content[2].ContentType, ContentType.Text);
            Assert.AreEqual(section2.Content[2].Text, "normal text continues");

            Assert.AreEqual(section2.Content[3].ContentType, ContentType.Text);
            Assert.AreEqual(section2.Content[3].Text, "and further");
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
            Assert.IsTrue(content.Children[0] != null);
            Assert.IsTrue(content.Children[1] != null);
            var section1 = content.Children[0];
            var section4 = content.Children[1];

            Assert.AreEqual(section1.Header.Text, "H1");
            Assert.AreEqual(section1.Content.Count, 0);
            Assert.AreEqual(section1.Children.Count, 2);
            Assert.IsTrue(section1.Children[0] != null);
            Assert.IsTrue(section1.Children[1] != null);

            var section2 = section1.Children[0];
            Assert.AreEqual(section2.Header.Text, "H2");
            Assert.AreEqual(section2.Content.Count, 0);
            Assert.AreEqual(section2.Children.Count, 1);
            Assert.IsTrue(section2.Children[0] != null);
            
            var section21 = section2.Children[0];
            Assert.AreEqual(section21.Header.Text, "H2.1");
            Assert.AreEqual(section21.Content.Count, 2); //skip textContent inspection
            Assert.AreEqual(section21.Children.Count, 0);

            var section3 = section1.Children[1];
            Assert.AreEqual(section3.Header.Text, "H3");
            Assert.AreEqual(section3.Content.Count, 2);
            Assert.AreEqual(section3.Children.Count, 0);
            
            Assert.AreEqual(section4.Header.Text, "H4");
            Assert.AreEqual(section4.Content.Count, 0);
            Assert.AreEqual(section4.Children.Count, 1);
            Assert.IsTrue(section4.Children[0] != null);

            var section5 = section4.Children[0];
            Assert.AreEqual(section5.Header.Text, "H5");
            Assert.AreEqual(section5.Content.Count, 2);
            Assert.AreEqual(section5.Children.Count, 0);
        }
    }
}
