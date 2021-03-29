using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
