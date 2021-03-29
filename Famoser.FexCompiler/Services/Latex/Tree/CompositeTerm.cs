using System.Collections.Generic;
using Famoser.FexCompiler.Services.Latex.Visitor;

namespace Famoser.FexCompiler.Services.Latex.Tree
{
    class CompositeTerm : Term
    {
        public List<Term> Terms { private set; get; }

        public CompositeTerm(List<Term> terms)
        {
            Terms = terms;
        }

        public override void Accept(TermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
