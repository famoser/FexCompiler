using Famoser.FexCompiler.Services.Latex.Visitor;

namespace Famoser.FexCompiler.Services.Latex.Tree
{
    class LeftRightTerm: Term
    {
        public LeftRightTerm(char connector, Term left, Term right)
        {
            Connector = connector;
            Left = left;
            Right = right;
        }

        public char Connector { private set; get; }
        public Term Left { private set; get; }
        public Term Right { private set; get; }

        public static bool IsConnector(char candidate)
        {
            return candidate == '^' || candidate == '_';
        }

        public override void Accept(TermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
