using Famoser.FexCompiler.Models.Document.Content.Base;

namespace Famoser.FexCompiler.Models.Document.Content
{
    public class Code : BaseContent
    {
        public Code(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}
