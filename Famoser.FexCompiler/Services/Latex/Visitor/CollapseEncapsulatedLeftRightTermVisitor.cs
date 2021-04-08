using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Services.Latex.Tree;

namespace Famoser.FexCompiler.Services.Latex.Visitor
{
    class CollapseEncapsulatedLeftRightTermVisitor : TermVisitor
    {
        public override void Visit(EncapsulatedTerm encapsulated)
        {
            if (encapsulated.Term is LeftRightTerm leftRight && leftRight.Left is RawTerm { Content: "" })
            {
                encapsulated.ShowEncapsulatorTerm = false;
            }

            base.Visit(encapsulated);
        }
    }
}
