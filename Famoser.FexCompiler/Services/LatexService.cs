using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Famoser.FexCompiler.Enum;
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

        private const int AfterParagraphSpacing = 3;
        private const int ParagraphOnParagraphSpacing = 5;
        private const int SectionOnParagraphSpacing = 8;

        public LatexService(StatisticModel statisticModel, MetaDataModel metaDataModel, List<BaseContent> content)
        {
            _statisticModel = statisticModel;
            _metaDataModel = metaDataModel;
            _content = content;
        }

        public string Process()
        {
            var path = PathHelper.GetAssemblyPath("Templates/template_Article.tex");
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

        private string ToLatex(BaseContent content, int level = 0)
        {
            var res = "";
            if (content is Section)
            {
                var section = (Section) content;
                var sectionName = "section";
                if (level == 1)
                    sectionName = "subsection";
                else if (level == 2)
                    sectionName = "subsubsection";
                else if (level == 3)
                    sectionName = "paragraph";
                else if (level == 4)
                    sectionName = "subparagraph";
                else if (level > 4)
                    sectionName = "subsubparagraph";

                res += "\\" + sectionName + "{" + ToLatex(section.Header) + " }\n";
                if (level > 2)
                {
                    //force line break after paragraph
                    res += "\\mbox{}\\\\\\vspace{" + AfterParagraphSpacing + "pt}";
                }

                //add section description
                res += ToLatex(section.TextContent);

                //add higher levels recursively
                for (var index = 0; index < section.Content.Count; index++)
                {
                    var baseContent = section.Content[index];
                    res += ToLatex(baseContent, level + 1);
                    var beforeSection = baseContent as Section;
                    var afterSection = index + 1 < section.Content.Count ? section.Content[index + 1] as Section : null;
                    if (beforeSection != null && afterSection != null && level > 1)
                    {
                        //if after is paragraph, need to add spacing
                        if (!afterSection.Content.Any())
                        {
                            //if before is paragraph too, only little spacing needed
                            if (!beforeSection.Content.Any())
                            {
                                res += "\\vspace{" + ParagraphOnParagraphSpacing + "pt}";
                            }
                            else
                            {
                                res += "\\vspace{" + SectionOnParagraphSpacing + "pt}";
                            }
                        }
                    }
                }
            }
            else if (content is Code)
            {
                var code = (Code) content;
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
            
            return content;
        }

        private string EscapeText(string text)
        {
            /*
             * & %  # _ { } ~ ^ \
             * */
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

            return text;
        }
    }
}
