using System;
using System.Collections.Generic;
using System.IO;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Models.TextRepresentation.Base;

namespace Famoser.FexCompiler.Helpers
{
    public class LatexHelper
    {
        public static string CreateLatex(Document document)
        {
            var path = PathHelper.GetAssemblyPath("Templates/template_Article.tex");
            var template = File.ReadAllText(path);

            template = template.Replace("TITLE", document.Title);
            template = template.Replace("AUTHOR", document.Author);
            template = template.Replace("GENERATED_AT", DateTime.Now.ToLongDateString());

            var content = ToLatex(document.Content, 0);

            template = template.Replace("CONTENT", content);
            return template;
        }

        private static string ToLatex(List<Content> contentList, int level)
        {
            var res = "";
            foreach (var content in contentList)
            {
                if (content is Paragraph)
                {
                    res += ToLatex((Paragraph)content);
                }
                if (content is Section)
                {
                    res += ToLatex((Section)content, level);
                }
            }
            return res;
        }

        private static string ToLatex(Section section, int level)
        {
            var sectionName = "section";
            if (level == 1)
                sectionName = "sub" + sectionName;
            else if (level == 2)
                sectionName = "subsub" + sectionName;
            else if (level == 3)
                sectionName = "paragraph";
            else if (level== 4)
                sectionName = "subparagraph";
            else if (level > 4)
                sectionName = "subsubparagraph";
            var content = "";
            content += "\\" + sectionName + "{" + ToLatex(section.Title, false) + " \\\\}\n";
            content += ToLatex(section.Content, level + 1);
            return content;
        }

        private static string ToLatex(List<Paragraph> paragraphs)
        {
            var content = "";
            foreach (var textNode in paragraphs)
            {
                content += ToLatex(textNode);
            }
            return content;
        }

        private static string ToLatex(Paragraph paragraph)
        {
            var content = "";
            if (paragraph.IsEnumeration)
            {
                content += "\\begin{itemize}\n";
                content += ToLatex(paragraph.LineNodes, "\\item ");
                content += "\\end{itemize}\n";
            }
            else
            {
                if (paragraph.ExtraIndentation)
                {
                    content += "\\hspace{0.5cm} ";
                }
                content += ToLatex(paragraph.LineNodes, "");
                content += "\n"; //\\* 
            }
            return content;
        }

        private static string ToLatex(List<LineNode> lines, string prefix)
        {
            var content = "";
            foreach (var textNode in lines)
            {
                content += prefix + ToLatex(textNode, true) + "\n";
            }
            return content;
        }

        private static string ToLatex(LineNode line, bool allowNewline)
        {
            var content = ToLatex(line.TextNodes);
            if (allowNewline)
                content += "\n";
            return content;
        }

        private static string ToLatex(TextNode textNode)
        {
            var content = "";
            if (textNode.TextType == TextType.Normal)
            {
                content += EscapeText(textNode.Text);
            }
            else if (textNode.TextType == TextType.Bold)
            {
                content += "\\textbf{" + EscapeText(textNode.Text) + "}";
            }
            content += " ";
            return content;
        }

        private static string ToLatex(List<TextNode> nodes)
        {
            var content = "";
            foreach (var textNode in nodes)
            {
                content += ToLatex(textNode);
            }
            return content;
        }

        private static string EscapeText(string text)
        {
            /*
             * & %  # _ { } ~ ^ \
             * */
            var replaces = new Dictionary<string, string>()
            {
                {"\\", "\\textbackslash "},
                {"→", "->" },
                {"∙", "*" },
                {"α", "\\textalpha " },
                {"β", "\\textbeta " },
                {"σ", "\\textsigma " },
                {"~", "\\textasciitilde "},
                {"^", "\\textasciicircum "},
                {">", "\\textgreater "},
                {"<", "\\textless "},
                {"*", "\\textasteriskcentered "},
                {"|", "\\textbar "},
                {"{", "\\textbraceleft "},
                {"}", "\\textbraceright "},
                {"$", "\\textdollar "},
                {"—", "\\textemdash "},
                {"“", "\\textquotedblleft "},
                {"”", "\\textquotedblright "},
                {"„",  "\\textquotedblleft "},
                {"‘",  "'"},
                {"‚",  ","} //<- this is not a comma: , (other UTF-8 code)
            };

            foreach (var replace in replaces)
            {
                text = text.Replace(replace.Key, replace.Value);
            }
            var escapes = new[] { "&", "%", "$", "#", "_", "{", "}" };
            foreach (var escape in escapes)
            {
                text = text.Replace(escape, "\\" + escape);
            }
            return text;
        }
    }
}
