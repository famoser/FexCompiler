using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Services.Latex.Tree;

namespace Famoser.FexCompiler.Services.Latex.Visitor
{
    class ExpandLeftRightTermVisitor : TermVisitor
    {
        public override void Visit(CompositeTerm composite)
        {
            var prefix = new CompositeTerm(new List<Term>());
            CompositeTerm suffix = null;

            for (int i = 0; i < composite.Terms.Count; i++)
            {
                if (composite.Terms[i] is DividerTerm)
                {
                    prefix = new CompositeTerm(new List<Term>());
                    suffix = null;
                }
                else if (composite.Terms[i] is LeftRightTerm newLeftRightTerm)
                {
                    composite.Terms.RemoveRange(i - prefix.Terms.Count, prefix.Terms.Count);
                    i -= prefix.Terms.Count;

                    if (newLeftRightTerm.Left is not RawTerm { Content: "" })
                    {
                        prefix.Terms.Add(newLeftRightTerm.Left);
                    }

                    suffix = new CompositeTerm(new List<Term>());
                    if (newLeftRightTerm.Right is not RawTerm { Content: "" })
                    {
                        suffix.Terms.Add(newLeftRightTerm.Right);
                    }

                    composite.Terms[i] = new LeftRightTerm(newLeftRightTerm.Connector, prefix, suffix);
                }
                else
                {
                    if (suffix != null)
                    {
                        suffix.Terms.Add(composite.Terms[i]);
                        composite.Terms.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        prefix.Terms.Add(composite.Terms[i]);
                    }
                }
            }

            for (int i = 0; i < composite.Terms.Count; i++)
            {
                if (composite.Terms[i] is LeftRightTerm leftRightTerm &&
                    leftRightTerm.Right is CompositeTerm rightComposite &&
                    leftRightTerm.Left is CompositeTerm leftComposite)
                {
                    // if either left or right empty, abort and go back to raw terms
                    if (leftComposite.Terms.Count == 0 || rightComposite.Terms.Count == 0)
                    {
                        composite.Terms[i] = new RawTerm(leftRightTerm.Connector.ToString());
                        composite.Terms.InsertRange(i + 1, rightComposite.Terms);
                        composite.Terms.InsertRange(i, leftComposite.Terms);
                        i += rightComposite.Terms.Count + leftComposite.Terms.Count;
                    }
                    else
                    {
                        // else avoid composite terms with single member
                        var newRight = rightComposite.Terms.Count == 1 ? rightComposite.Terms.Single() : rightComposite;
                        var newLeft = leftComposite.Terms.Count == 1 ? leftComposite.Terms.Single() : leftComposite;
                        composite.Terms[i] = new LeftRightTerm(leftRightTerm.Connector, newLeft, newRight);
                    }
                }
            }

            base.Visit(composite);
        }
    }
}
