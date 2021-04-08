using System;
using Famoser.FexCompiler.Services.Latex.Visitor;

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
            return candidate == ',' || candidate == ' ' || candidate == '|';
        }

        public override void Accept(TermVisitor visitor)
        {
            visitor.Visit(this);
        }

        public bool RequiresMathMode()
        {
            return Divider == '|';
        }
    }
}
