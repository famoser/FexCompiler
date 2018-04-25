namespace Famoser.FexCompiler.Models.Document.TextRepresentation
{
    public class Code
    {
        public Code(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}
