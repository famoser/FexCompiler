using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Services.Latex.Tree;

namespace Famoser.FexCompiler.Services.Latex.Visitor
{
    class HideEncapsulatorsVisitor : TermVisitor
    {
        public override void Visit(LeftRightTerm leftRightTerm)
        {
            if (leftRightTerm.Right is EncapsulatedTerm {Encapsulator: '{'} encapsulated)
            {
                encapsulated.ShowEncapsulatorTerm = false;
            }

            base.Visit(leftRightTerm);
        }
    }
}
