using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.Content.Base;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    public class LatexService : IProcessService<string>
    {
        private readonly StatisticModel _statisticModel;
        private readonly MetaDataModel _metaDataModel;
        private readonly List<BaseContent> _content;

        private const int ParagraphOnParagraphSpacing = 5;

        public LatexService(StatisticModel statisticModel, MetaDataModel metaDataModel, List<BaseContent> content)
        {
            _statisticModel = statisticModel;
            _metaDataModel = metaDataModel;
            _content = content;
        }

        public string Process()
        {
            var path = Path.Combine(PathHelper.GetAssemblyPath(), "Templates/template_Article.tex");
            var template = File.ReadAllText(path);

            template = template.Replace("TITLE", _metaDataModel.Title);
            template = template.Replace("AUTHOR", _metaDataModel.Author);
            template = template.Replace("CHARACTER_COUNT", _statisticModel.CharacterCount.ToString());
            template = template.Replace("WORD_COUNT", _statisticModel.WordCount.ToString());
            template = template.Replace("LINE_COUNT", _statisticModel.LineCount.ToString());

            var content = "";
            foreach (var baseContent in _content)
            {
                content += ToLatex(baseContent);
            }

            template = template.Replace("CONTENT", content);
            return template;
        }

        private string ToLatex(BaseContent content, int level = 0, bool paragraphsDisabled = false)
        {
            var res = "";
            if (content is Section)
            {
                var section = (Section)content;

                if (section.Content.Any() || paragraphsDisabled)
                {
                    var sectionName = "section";
                    //max sub is sub_sub_sub_sub_section (no _, just here for easier counting)
                    for (int i = 0; i < level && i < 4; i++)
                    {
                        sectionName = "sub" + sectionName;
                    }

                    res += "\\" + sectionName + "{" + ToLatex(section.Header) + "}\n";
                }
                else
                {
                    //use bold only when no sub content
                    res += "\\textbf{" + ToLatex(section.Header) + "}\\\\\n";
                }

                if (section.TextContent.Any())
                {
                    //add section description
                    res += ToLatex(section.TextContent);

                    //add spacer if bold is following afterwards
                    if (section.Content.Any() &&
                        section.Content[0] is Section &&
                        !((Section)section.Content[0]).Content.Any())
                    {
                        res += "\\vspace{" + ParagraphOnParagraphSpacing + "pt}\n";
                    }
                }


                //disable the collapsing to paragraph if section before was not a paragraph
                //this ensures the user is not confused (a "chapter" could then look like as it would belong to the chapter before)
                bool onlyParagraphs = true;
                foreach (var baseContent in section.Content)
                {
                    var mySection = baseContent as Section;
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
            }
            else if (content is Code)
            {
                var code = (Code)content;
                res += "\\onecolumn{}\n\\begin{verbatim}\n";
                res += code.Text;
                res += "\\end{verbatim}\n\\twocolumn{}\n";
            }
            else
            {
                Debug.Fail("unknown content type");
            }

            return res;
        }

        private string ToLatex(List<LineNode> lines)
        {
            var content = "";
            foreach (var lineNode in lines)
            {
                content += ToLatex(lineNode);
                //latex newline + OS newline
                content += "\\\\ " + Environment.NewLine;
            }
            return content;
        }

        private string ToLatex(LineNode line)
        {
            var content = "";
            foreach (var textNode in line.TextNodes)
            {
                if (textNode.TextType == TextType.Normal)
                {
                    content += EscapeText(textNode.Text);
                }
                else if (textNode.TextType == TextType.Bold)
                {
                    content += "\\textbf{" + EscapeText(textNode.Text) + "}";
                }
                content += " ";
            }

            if (content.Length > 0)
                content = content.Substring(0, content.Length - 1);

            return content;
        }

        private string EscapeText(string text)
        {
            //first round replace
            var replaces = new Dictionary<string, string>()
            {
                {"$", "\\textdollar "},
                {"\\", "\\textbackslash "},
                {"∙", "*" },
                {"→", "$\\to$" },
                {"->", "$\\to$" },
                {"=>", "$\\Rightarrow$" },
                {"<=>", "$\\Leftrightarrow" },
                {">=", "$\\ge$"},
                {"<=", " $\\le$"},
                {"&",  "\\&"},
                {"%",  "\\%"},
                {"#",  "\\#"},
                {"_",  "\\_"},
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
                {"^", "\\textasciicircum"},
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

            return text;
        }
    }
}
