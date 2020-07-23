using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var line = EscapeText(de);

            var result = "";
            var prefix = "";
            var activeWord = "";
            char? combineChar = null;
            bool blocked = false;
            foreach (var entry in line)
            {
                if (
                    entry == ' ' || entry == ',' ||
                    entry == '(' || entry == ')' ||
                    entry == '[' || entry == ']' ||
                    entry == '{' || entry == '}'
                )
                {
                    result += PrintCombineChar(combineChar, activeWord, prefix, entry);
                    combineChar = null;
                    activeWord = "";
                    blocked = false;
                    continue;
                }

                if ((entry == '_' || entry == '^') && !blocked)
                {
                    if (combineChar == null)
                    {
                        prefix = activeWord;
                        activeWord = "";
                        combineChar = entry;
                    }
                    else
                    {
                        activeWord = prefix + combineChar;
                        combineChar = null;
                        blocked = true;
                    }
                }
                else
                {
                    activeWord += entry;
                }
            }

            result += PrintCombineChar(combineChar, activeWord, prefix, null);

            return result;
        }

        private static string PrintCombineChar(char? combineChar, string activeWord, string prefix, char? entry)
        {
            var replaces = new Dictionary<char, string>()
            {
                {'_',  "\\_"},
                {'^', "\\textasciicircum"}
            };

            string result;
            if (combineChar == null)
            {
                result = (activeWord + entry);
            }
            else if (activeWord.Length > 0)
            {
                result = "$" + prefix + combineChar + "{" + activeWord + "}" + "$" + entry;
            }
            else
            {
                result = prefix + replaces[combineChar.Value] + activeWord + entry;
            }

            return result;
        }

        private string EscapeText(string text)
        {
            //first round replace
            var replaces = new Dictionary<string, string>()
            {
                {"\\", "\\textbackslash "},
                {"$", "\\textdollar "},
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
                {"‚",  ","} //<- this is not a comma: , (other UTF-8 code)
            };
            foreach (var replace in replaces)
            {
                text = text.Replace(replace.Key, replace.Value);
            }

            //replaces where hspace afterwards is needed
            var textReplaces = new Dictionary<string, string>()
            {
                {"{", "\\textbraceleft"},
                {"}", "\\textbraceright"},
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
                {"„",  "\\textquotedblleft"}
            };

            foreach (var textReplace in textReplaces)
            {
                text = text.Replace(textReplace.Key + " ", textReplace.Value + " " + "VSPACE_PLACEHOLDER");
                text = text.Replace(textReplace.Key, textReplace.Value + " ");
            }

            text = text.Replace("VSPACE_PLACEHOLDER", "\\hspace{3pt}");


            //second round replace
            var replaces2 = new Dictionary<string, string>()
            {
                {"[", "{[}"},
                {"]", "{]}"}
            };
            foreach (var replace in replaces2)
            {
                text = text.Replace(replace.Key, replace.Value);
            }

            //text replaces
            var replaces3 = new Dictionary<string, string>()
            {
                {"not_element_of", "$\\notin$"},
                {"element_of", "$\\in$"},
                {"exists", "$\\in$"},
                {"for_all", "$\\in$"}
            };
            foreach (var replace in replaces3)
            {
                text = text.Replace(replace.Key, replace.Value);
            }

            return text;
        }

        private bool IsValidExponent(string content)
        {
            var maxChars = 3;
            var maxNumbers = 10;
            for (var index = 0; index < content.Length; index++)
            {
                var item = content[index];
                if (item >= 'a' && item <= 'z' || //alpha 
                    item >= 'A' && item <= 'Z') //ALPHA
                {
                    if (maxChars-- <= 0)
                    {
                        return false;
                    }
                }
                else if (item >= '0' && item <= '9') //numbers
                {
                    if (maxNumbers-- <= 0)
                    {
                        return false;
                    }
                }
                else if (item == '-')
                {
                    if (index > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidVariable(string content)
        {
            foreach (var item in content)
            {
                if (
                    item >= 'a' && item <= 'z' || //alpha 
                    item >= 'A' && item <= 'Z' || //ALPHA
                    item >= '0' && item <= '9' ||
                    item >= '_')
                {
                    //valid
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private string EscapeVariable(string content)
        {
            return content.Replace("_", "\\_");
        }

        private bool ValidSubscriptChar(char entry)
        {
            return entry >= 'a' && entry <= 'z' || //alpha 
                   entry >= 'A' && entry <= 'Z' || //ALPHA
                   entry >= '0' && entry <= '9';
        }
    }
}
