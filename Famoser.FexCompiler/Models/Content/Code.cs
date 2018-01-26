using Famoser.FexCompiler.Models.Content.Base;

namespace Famoser.FexCompiler.Models.Content
{
    public class Code : BaseContent
    {
        public Code(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
