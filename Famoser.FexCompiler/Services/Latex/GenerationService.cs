using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services.Latex
{
    public class GenerationService : IProcessService<string>
    {
        private readonly StatisticModel _statisticModel;
        private readonly MetaDataModel _metaDataModel;
        private readonly List<Section> _content;
        private string _renderedContent;
        private string _templateName = "Summary";

        private const int ParagraphOnParagraphSpacing = 5;

        public GenerationService(StatisticModel statisticModel, MetaDataModel metaDataModel, List<Section> content)
        {
            _statisticModel = statisticModel;
            _metaDataModel = metaDataModel;
            _content = content;
        }

        public void SetTemplateName(string templateName)
        {
            _templateName = templateName;
        }

        public string Process()
        {
            var path = Path.Combine(PathHelper.GetAssemblyPath(), "Templates/template_" + _templateName + ".tex");
            var template = File.ReadAllText(path);

            template = template.Replace("TITLE", _metaDataModel.Title);
            template = template.Replace("AUTHOR", _metaDataModel.Author);
            template = template.Replace("CHARACTER_COUNT", _statisticModel.CharacterCount.ToString());
            template = template.Replace("WORD_COUNT", _statisticModel.WordCount.ToString());
            template = template.Replace("LINE_COUNT", _statisticModel.LineCount.ToString());

            if (_renderedContent == null)
            {
                _renderedContent = "";
                foreach (var baseContent in _content)
                {
                    _renderedContent += ToLatex(baseContent);
                }
            }

            template = template.Replace("CONTENT", _renderedContent);
            return template;
        }

        private string ToLatex(Section section, int level = 0)
        {
            var res = "";

            // print title
            var sectionName = "section";
            for (var i = 0; i < level && i < 4; i++)
            {
                sectionName = "sub" + sectionName;
            }
            var title = "\\" + sectionName + "{" + ToLatex(section.Header.Text) + "}\n";
            res += title;

            // print content
            var content = ToLatex(section.Content);
            res += content;

            // if children have no children, output last level: paragraphs
            if (section.Children.All(c => !c.Children.Any()))
            {
                var paragraphSpacer = "\\vspace{" + ParagraphOnParagraphSpacing + "pt}\n";
                res += paragraphSpacer;

                foreach (var sectionChild in section.Children)
                {
                    res += "\\textbf{" + ToLatex(sectionChild.Header.Text) + "}\\\\\n";
                    res += ToLatex(sectionChild.Content);
                    res += paragraphSpacer;
                }
            }
            else
            {
                // else recurse
                foreach (var sectionChild in section.Children)
                {
                    res += ToLatex(sectionChild, level + 1);
                }
            }

            return res;
        }

        private string ToLatex(List<Content> lines)
        {
            var content = "";
            foreach (var lineNode in lines)
            {
                if (lineNode.ContentType == ContentType.Text)
                {
                    //write text
                    var textContent = ToLatex(lineNode.Text);
                    if (textContent != "")
                    {
                        //latex newline + OS newline
                        content += textContent + "\\\\ " + Environment.NewLine;
                    }

                }
                else if (lineNode.ContentType == ContentType.Code)
                {
                    content += "\\begin{verbatim}\n" +
                               lineNode.Text +
                               "\n\\end{verbatim}";
                }
            }

            return content;
        }

        private string ToLatex(string de)
        {
            var line = de;

            /*
             * small statemachine for parsing single line.
             *
             * 0 for valid word chars (initial)
             * 1 for valid word (at least 3 valid word chars)
             * 2 for valid combine char
             * 3 for valid exponent
             *
             * 0->0 on valid word char
             * 0->1 on valid word char
             *
             * 1->2 on combine char
             * 1->1 on valid word char
             * 1->-1 else
             *
             * 2->3 on valid exponent
             * 2->-1 else
             *
             * 3->3 on valid exponent
             * 3->-1 else; if end char then OUTPUT
             */

            var state = 0;
            var invalidPrefix = "";
            var variable = "";
            char? combineChar = null;
            var exponent = "";

            var result = "";

            foreach (var entry in line)
            {
                switch (state)
                {
                    case 0:
                        variable += entry;
                        if (!IsVariableChar(entry))
                        {
                            invalidPrefix += variable;
                            variable = "";
                        }
                        else if (variable.Length >= 1)
                        {
                            state = 1;
                        }

                        break;
                    case 1:
                        if (entry == '_' || entry == '^')
                        {
                            combineChar = entry;
                            state = 2;
                        }
                        else if (IsVariableChar(entry))
                        {
                            variable += entry;
                        }
                        else
                        {
                            invalidPrefix += variable + entry;
                            variable = "";
                            state = 0;
                        }

                        break;
                    case 2:
                        exponent += entry;
                        if (IsVariableChar(entry))
                        {
                            state = 3;
                        }
                        else
                        {
                            invalidPrefix += variable + combineChar + exponent;
                            variable = "";
                            combineChar = null;
                            exponent = "";
                            state = 0;
                        }

                        break;
                    case 3:
                        if (IsVariableChar(entry))
                        {
                            exponent += entry;
                        }
                        else
                        {
                            if (IsEndChar(entry) && !ReservedWord(variable, combineChar, exponent))
                            {
                                result += EscapeText(invalidPrefix);
                                result += "${" + variable + "}" + combineChar + "{" + exponent + "}$";
                                invalidPrefix = entry.ToString();
                            }
                            else
                            {
                                invalidPrefix += variable + combineChar + exponent + entry;
                            }

                            state = 0;
                            variable = "";
                            combineChar = null;
                            exponent = "";
                        }

                        break;
                }
            }

            if (combineChar != null && exponent.Length > 0)
            {
                result += EscapeText(invalidPrefix);
                result += "${" + variable + "}" + combineChar + "{" + exponent + "}$";
            }
            else
            {

                invalidPrefix += variable;
                if (combineChar != null)
                {
                    invalidPrefix += combineChar + exponent;
                }

                result += EscapeText(invalidPrefix);
            }

            return result;
        }

        private bool ReservedWord(string variable, char? combineChar, string exponent)
        {
            var word = variable + combineChar + exponent;
            return word == "sum_of" || word == "for_all" || word == "element_of";
        }

        private string EscapeText(string text)
        {
            // replace characters which are used in later replaces
            var replaces = new Dictionary<string, string>()
            {
                {"\\", "\\textbackslash"},
                {"$", "\\textdollar"},
                {"not_element_of", "$\\notin$"},
                {"element_of", "$\\in$"},
                {"exists", "$\\in$"},
                {"for_all", "$\\in$"},
                {"sum_of", "$\\sum$"},
                {"_", "\\_"},
                {"{", "\\textbraceleft"},
                {"}", "\\textbraceright"},
                {"[", "{[}"},
                {"]", "{]}"},
                {"∙", "*" },
                {"→", "$\\rightarrow$" },
                {"<->", "$\\leftrightarrow$" },
                {"->", "$\\rightarrow$" },
                {"<-", "$\\leftarrow$" },
                {"<=>", "$\\Leftrightarrow$" },
                {"=>", "$\\Rightarrow$" },
                {">=", "$\\ge$"},
                {"<=", " $\\le$"},
                {"&",  "\\&"},
                {"%",  "\\%"},
                {"#",  "\\#"},
                {"°", " \\degree"},
                {"‘",  "'"},
                {"‚",  ","}, //<- this is not a comma: , (other UTF-8 code)
                {"α", "\\textalpha" },
                {"β", "\\textbeta" },
                {"σ", "\\textsigma" },
                {"~", "\\textasciitilde"},
                {">", "\\textgreater"},
                {"<", "\\textless"},
                {"*", "\\textasteriskcentered"},
                {"|", "\\textbar"},
                {"—", "\\textemdash"},
                {"“", "\\textquotedblleft"},
                {"”", "\\textquotedblright"},
                {"„",  "\\textquotedblleft"},
                {"^", "\\textasciicircum"}
            };

            foreach (var textReplace in replaces)
            {
                var replace = textReplace.Value;
                if (replace.Contains("text"))
                {
                    replace += " VSPACEPLACEHOLDER";
                }

                text = text.Replace(textReplace.Key, replace);
            }

            text = text.Replace(" VSPACEPLACEHOLDER ", " \\hspace{0pt} ");
            text = text.Replace(" VSPACEPLACEHOLDER", " ");

            return text;
        }

        private bool IsEndChar(char entry)
        {
            return entry == ' ' || entry == ',' ||
                   entry == '(' || entry == ')' ||
                   entry == '[' || entry == ']' ||
                   entry == '|' ||
                   entry == '{' || entry == '}';
        }

        private bool IsVariableChar(char item)
        {
            return item >= 'a' && item <= 'z' || //alpha 
                   item >= 'A' && item <= 'Z' || //ALPHA
                   item >= '0' && item <= '9'; // numbers
        }
    }
}
