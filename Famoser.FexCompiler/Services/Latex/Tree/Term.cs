using Famoser.FexCompiler.Services.Latex.Visitor;

namespace Famoser.FexCompiler.Services.Latex.Tree
{
    abstract class Term
    {
        public abstract void Accept(TermVisitor visitor);
    }
}
