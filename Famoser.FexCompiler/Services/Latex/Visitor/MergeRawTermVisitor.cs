using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Services.Latex.Tree;

namespace Famoser.FexCompiler.Services.Latex.Visitor
{
    class MergeRawTermVisitor : TermVisitor
    {
        public override void Visit(CompositeTerm composite)
        {
            // remove blank raws
            for (int i = 0; i < composite.Terms.Count; i++)
            {
                if (composite.Terms[i] is RawTerm { Content: "" })
                {
                    composite.Terms.RemoveAt(i);
                    i--;
                }
            }

            // merge raws immediately following each other
            for (int i = 0; i < composite.Terms.Count - 1; i++)
            {
                if (composite.Terms[i] is RawTerm raw && composite.Terms[i + 1] is RawTerm nextRaw)
                {
                    composite.Terms[i] = new RawTerm(raw.Content + nextRaw.Content);
                    composite.Terms.RemoveAt(i + 1);
                }
            }

            base.Visit(composite);
        }
    }
}
