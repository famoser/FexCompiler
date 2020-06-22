using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Models.Document.Content;
using Famoser.FexCompiler.Models.Document.TextRepresentation;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    public class LatexService : IProcessService<string>
    {
        private readonly StatisticModel _statisticModel;
        private readonly MetaDataModel _metaDataModel;
        private readonly List<Section> _content;
        private string _renderedContent;
        private string _templateName = "Summary";

        private const int ParagraphOnParagraphSpacing = 5;

        public LatexService(StatisticModel statisticModel, MetaDataModel metaDataModel, List<Section> content)
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

        private string ToLatex(Section section, int level = 0, bool paragraphsDisabled = false)
        {
            var res = "";

            if (section.Content.Any() || paragraphsDisabled)
            {
                var sectionName = "section";
                //max sub is sub_sub_sub_sub_section (no _, just here for easier counting)
                for (int i = 0; i < level && i < 4; i++)
                {
                    sectionName = "sub" + sectionName;
                }

                res += "\\" + sectionName + "{" + ToLatex(section.Header.TextNodes) + "}\n";
            }
            else
            {
                //use bold only when no sub content
                res += "\\textbf{" + ToLatex(section.Header.TextNodes) + "}\\\\\n";
            }

            if (section.TextContent.Any())
            {
                //add section description
                res += ToLatex(section.TextContent);

                //add spacer if bold is following afterwards
                if (section.Content.Any() &&
                    !section.Content[0].Content.Any())
                {
                    res += "\\vspace{" + ParagraphOnParagraphSpacing + "pt}\n";
                }
            }


            //disable the collapsing to paragraph if section before was not a paragraph
            //this ensures the user is not confused (a "chapter" could then look like as it would belong to the chapter before)
            bool onlyParagraphs = true;
            foreach (var baseContent in section.Content)
            {
                var mySection = baseContent;
                if (mySection != null && mySection.Content.Any())
                {
                    onlyParagraphs = false;
                }
            }

            //add content recursively
            foreach (var baseContent in section.Content)
            {
                res += ToLatex(baseContent, level + 1, !onlyParagraphs);

                //add spacing if paragraphs
                if (onlyParagraphs)
                {
                    res += "\\vspace{" + ParagraphOnParagraphSpacing + "pt}\n";
                }
            }

            return res;
        }

        private string ToLatex(List<LineNode> lines)
        {
            var content = "";
            foreach (var lineNode in lines)
            {
                //write text
                var textContent = ToLatex(lineNode.TextNodes);
                if (textContent != "")
                    //latex newline + OS newline
                    content += textContent + "\\\\ " + Environment.NewLine;

                //write code (typically only one property is not empty / not null)
                if (lineNode.CodeNode != null)
                {
                    content += "\\begin{verbatim}\n" +
                               lineNode.CodeNode.Text +
                               "\n\\end{verbatim}";
                }
            }
            return content;
        }

        private string ToLatex(List<TextNode> textNodes)
        {
            if (textNodes == null)
                return "";

            var content = "";
            foreach (var textNode in textNodes)
            {
                switch (textNode.TextType)
                {
                    case TextType.Bold:
                        content += "\\textbf{" + ToLatex(textNode.Text) + "}";
                        break;
                    default:
                        content += ToLatex(textNode.Text);
                        break;
                }

                content += " ";
            }

            if (content.Length > 0)
                content = content.Substring(0, content.Length - 1);

            return content;
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
                {"not\\_element\\_of", "$\\not\\in$"},
                {"element\\_of", "$\\in$"}
            };
            foreach (var replace in replaces3)
            {
                text = text.Replace(replace.Key, replace.Value);
            }

            return text;
        }
    }
}
