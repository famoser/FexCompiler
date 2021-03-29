using System;
using System.Collections.Generic;
using System.Linq;

namespace Famoser.FexCompiler.Services.Latex.Tree
{
    public class TextToLatexCompiler
    {
        public string Compile(string text)
        {
            var tree = TextToTerm(text);
            tree = AddLeftRightTerms(tree);
            ExpandLeftRightTerms(tree);

            return TermToLatex(tree);
        }

        private void ExpandLeftRightTerms(Term term)
        {
            if (term is not CompositeTerm composite)
            {
                return;
            }

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
                    if (newLeftRightTerm.Left is not RawTerm { Content: "" })
                    {
                        prefix.Terms.Add(newLeftRightTerm.Left);
                    }

                    composite.Terms.RemoveRange(i - prefix.Terms.Count, prefix.Terms.Count);
                    i -= prefix.Terms.Count;

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
                if (composite.Terms[i] is LeftRightTerm { Right: CompositeTerm rightComposite } leftRightTerm && rightComposite.Terms.Count == 1)
                {
                    var newRight = rightComposite.Terms.Single();
                    composite.Terms[i] = new LeftRightTerm(leftRightTerm.Connector, leftRightTerm.Left, newRight);
                }
            }
        }

        private Term TextToTerm(string text)
        {
            var terms = new List<Term>();

            var currentText = "";
            var encapsulationText = "";
            char? encapsulator = null;
            int encapsulationDepth = 0;
            for (var i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];

                if (encapsulator != null)
                {
                    if (EncapsulatedTerm.IsEncapsulatorEnd(currentChar, encapsulator.Value) && encapsulationDepth-- == 0)
                    {
                        var innerTerm = TextToTerm(encapsulationText);
                        terms.Add(new EncapsulatedTerm(encapsulator.Value, innerTerm));

                        encapsulator = null;
                        encapsulationText = "";
                        encapsulationDepth = 0;
                    }
                    else
                    {
                        if (currentChar == encapsulator.Value)
                        {
                            encapsulationDepth++;
                        }
                        encapsulationText += currentChar;
                    }

                    continue;
                }

                if (EncapsulatedTerm.IsEncapsulatorStart(currentChar))
                {
                    terms.Add(new RawTerm(currentText));
                    currentText = "";

                    encapsulator = currentChar;
                    continue;
                }

                if (DividerTerm.IsDivider(currentChar))
                {
                    terms.Add(new RawTerm(currentText));
                    currentText = "";

                    terms.Add(new DividerTerm(currentChar));

                    continue;
                }

                currentText += currentChar;
            }

            if (encapsulator != null)
            {
                var innerTerm = TextToTerm(encapsulationText);
                terms.Add(new EncapsulatedTerm(encapsulator.Value, innerTerm));
            }

            if (currentText.Length > 0)
            {
                terms.Add(new RawTerm(currentText));
            }

            return terms.Count == 1 ? terms.Single() : new CompositeTerm(terms);
        }

        private Term AddLeftRightTerms(Term term)
        {
            switch (term)
            {
                case CompositeTerm composite:
                    var parsedTerms = composite.Terms.Select(AddLeftRightTerms);
                    return new CompositeTerm(parsedTerms.ToList());
                case EncapsulatedTerm encapsulated:
                    return new EncapsulatedTerm(encapsulated.Encapsulator, AddLeftRightTerms(encapsulated.Term));
                case RawTerm raw:
                    return ParseLeftRightTerms(raw.Content);
                default:
                    return term;
            }
        }

        private Term ParseLeftRightTerms(string text)
        {
            var prefix = "";
            for (int i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];

                if (LeftRightTerm.IsConnector(currentChar))
                {
                    var suffix = ParseLeftRightTerms(text.Substring(i + 1));
                    return new LeftRightTerm(currentChar, new RawTerm(prefix), suffix);
                }

                prefix += currentChar;
            }

            return new RawTerm(prefix);
        }

        private string TermToLatex(Term tree, bool mathMode = false)
        {
            switch (tree)
            {
                case CompositeTerm composite:
                    var latexTerms = composite.Terms.Select(t => TermToLatex(t, mathMode));
                    return string.Join("", latexTerms);
                case DividerTerm divider:
                    return divider.Divider.ToString();
                case EncapsulatedTerm encapsulated:
                    var left = encapsulated.Encapsulator;
                    var right = EncapsulatedTerm.GetEncapsulatorEnd(left);

                    return EscapeString(left.ToString(), mathMode) +
                           TermToLatex(encapsulated.Term, mathMode) +
                           EscapeString(right.ToString(), mathMode);
                case LeftRightTerm leftRightTerm:
                    var leftLatex = TermToLatex(leftRightTerm.Left, true);
                    leftLatex = leftLatex.Length > 1 ? "{" + leftLatex + "}" : leftLatex;

                    var rightLatex = TermToLatex(leftRightTerm.Right, true);
                    rightLatex = rightLatex.Length > 1 ? "{" + rightLatex + "}" : rightLatex;

                    var content = leftLatex + leftRightTerm.Connector + rightLatex;
                    return mathMode ? content : "$" + content + "$";
                case RawTerm raw:
                    return EscapeString(raw.Content, mathMode);
            }

            throw new Exception("Unknown node " + nameof(tree));
        }

        private string EscapeString(string text, bool mathMode)
        {
            if (text.StartsWith("\\") && !mathMode)
            {
                text = "$" + text + "$";
                mathMode = true;
            }

            var mathShortcuts = new Dictionary<string, string>()
            {
                {"<->", "\\leftrightarrow"},
                {"->", "\\rightarrow"},
                {"<-", "\\leftarrow"},
                {"<=>", "\\Leftrightarrow"},
                {"=>", "\\Rightarrow"},
                {">=", "\\ge"},
                {"<=", "\\le"},
            };

            foreach (var mathShortcut in mathShortcuts)
            {
                var replace = mathMode ? mathShortcut.Value : "$" + mathShortcut.Value + "$";
                text = text.Replace(mathShortcut.Key, replace);
            }

            if (mathMode)
            {
                // replace characters which are used in later replaces
                var textCharacterReplaces = new Dictionary<string, string>()
                {
                    {"{", "\\{"},
                    {"}", "\\}"},
                    {"α", "\\alpha"},
                    {"β", "\\beta"},
                    {"σ", "\\sigma"},
                    {"~", "\\textasciitilde"},
                    {"^", "\\hat{}"}
                };

                foreach (var textCharacterReplace in textCharacterReplaces)
                {
                    text = text.Replace(textCharacterReplace.Key, textCharacterReplace.Value);
                }
            }
            else
            {
                // replace characters which are used in later replaces
                var textCharacterReplaces = new Dictionary<string, string>()
                {
                    {"{", "\\textbraceleft"},
                    {"}", "\\textbraceright"},
                    {"α", "\\textalpha"},
                    {"β", "\\textbeta"},
                    {"σ", "\\textsigma"},
                    {"~", "\\textasciitilde"},
                    {">", "\\textgreater"},
                    {"<", "\\textless"},
                    {"*", "\\textasteriskcentered"},
                    {"|", "\\textbar"},
                    {"—", "\\textemdash"},
                    {"“", "\\textquotedblleft"},
                    {"”", "\\textquotedblright"},
                    {"„", "\\textquotedblleft"},
                    {"^", "\\textasciicircum"}
                };

                foreach (var textCharacterReplace in textCharacterReplaces)
                {
                    var replace = textCharacterReplace.Value + " \\hspace{0pt}";
                    text = text.Replace(textCharacterReplace.Key, replace);
                }
            }


            // replace characters which are used in later replaces
            var specialCharacterReplaces = new Dictionary<string, string>()
            {
                {"_", "\\_"},
                {"[", "{[}"},
                {"]", "{]}"},
                {"∙", "*"},
                {"&", "\\&"},
                {"%", "\\%"},
                {"#", "\\#"},
                {"°", " \\degree"},
                {"‘", "'"},
                {"‚", ","}, //<- this is not a comma: , (other UTF-8 code)
            };

            foreach (var specialCharacterReplace in specialCharacterReplaces)
            {
                text = text.Replace(specialCharacterReplace.Key, specialCharacterReplace.Value);
            }

            return text;
        }
    }
}
