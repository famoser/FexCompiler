using Famoser.FexCompiler.Models.ContentTypes.Base;

namespace Famoser.FexCompiler.Models.ContentTypes
{
    public class Code : Content
    {
        public Code(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
