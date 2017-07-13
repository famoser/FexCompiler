using System;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FexCompiler.Test
{
    [TestClass]
    public class TestFexHelper
    {
        [TestMethod]
        public void TestSimpleFex()
        {
            var fileContent = TestHelper.GetInputFile("simple.fex");
            var configModel = TestHelper.GetConfigModel();
            var doc = FexHelper.ParseDocument(fileContent.ToList(), "test", configModel);

            //check h1 section
            Assert.IsTrue(doc != null);
            Assert.IsTrue(doc.Content.Count == 1);
            Assert.IsTrue(doc.Content[0] is Section);

            var h1Section = (Section)doc.Content[0];
            //check H1 header
            Assert.IsTrue(h1Section.Title?.TextNodes?.Count == 1);
            Assert.IsTrue(h1Section.Title.TextNodes[0].TextType == TextType.Normal);
            Assert.IsTrue(h1Section.Title.TextNodes[0].Text == "H1");

            //check h1 content
            Assert.IsTrue(h1Section?.Content?.Count == 1);
            Assert.IsTrue(h1Section?.Content[0] is Section);

            var h2Section = (Section)h1Section.Content[0];
            //check H2 header
            Assert.IsTrue(h2Section.Title?.TextNodes?.Count == 1);
            Assert.IsTrue(h2Section.Title.TextNodes[0].TextType == TextType.Normal);
            Assert.IsTrue(h2Section.Title.TextNodes[0].Text == "H2");

            //check h2 content
            Assert.IsTrue(h2Section?.Content?.Count == 2);
            Assert.IsTrue(h2Section?.Content[0] is Paragraph);
            Assert.IsTrue(h2Section?.Content[1] is Paragraph);

            var paragraph0 = (Paragraph)h2Section.Content[0];
            var paragraph1 = (Paragraph)h2Section.Content[1];

            //check paragraph0
            Assert.IsTrue(paragraph0.LineNodes.Count == 1);
            Assert.IsTrue(paragraph0.LineNodes[0].TextNodes.Count == 1);
            Assert.IsTrue(paragraph0.LineNodes[0].TextNodes[0].Text == "hallo");

            //check paragraph1
            Assert.IsTrue(paragraph1.LineNodes?.Count == 1);
            Assert.IsTrue(paragraph1.LineNodes[0].TextNodes?.Count == 1);
            Assert.IsTrue(paragraph1.LineNodes[0].TextNodes[0].Text == "welt");
        }
    }
}
