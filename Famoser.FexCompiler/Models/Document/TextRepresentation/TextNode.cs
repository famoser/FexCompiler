using Famoser.FexCompiler.Enum;

namespace Famoser.FexCompiler.Models.Document.TextRepresentation
{
    public class TextNode
    {
        public TextNode(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
        public TextType TextType { get; set; }
    }
}
