using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Services.Latex.Tree;

namespace Famoser.FexCompiler.Services.Latex.Visitor
{
    abstract class TermVisitor
    {
        public virtual void Visit(CompositeTerm composite)
        {
            composite.Terms.ForEach(t => t.Accept(this));
        }

        public virtual void Visit(DividerTerm dividerTerm)
        {
        }

        public virtual void Visit(EncapsulatedTerm encapsulatedTerm)
        {
            encapsulatedTerm.Term.Accept(this);
        }

        public virtual void Visit(LeftRightTerm leftRightTerm)
        {
            leftRightTerm.Left.Accept(this);
            leftRightTerm.Right.Accept(this);
        }

        public virtual void Visit(RawTerm rawTerm)
        {
        }
    }
}
