using System.Collections.Generic;

namespace Famoser.FexCompiler.Services.Latex.Tree
{
    class CompositeTerm : Term
    {
        public List<Term> Terms { private set; get; }

        public CompositeTerm(List<Term> terms)
        {
            Terms = terms;
        }
    }
}
