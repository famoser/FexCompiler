namespace Famoser.FexCompiler.Services.Latex.Tree
{
    class RawTerm: Term
    {
        public RawTerm(string content)
        {
            Content = content;
        }

        public string Content { private set; get; }
    }
}
