namespace Famoser.FexCompiler.Services.Latex.Tree
{
    class DividerTerm : Term
    {
        public DividerTerm(char divider)
        {
            Divider = divider;
        }

        public char Divider { private set; get; }

        public static bool IsDivider(char candidate)
        {
            return candidate == ',' || candidate == ' ';
        }
    }
}
