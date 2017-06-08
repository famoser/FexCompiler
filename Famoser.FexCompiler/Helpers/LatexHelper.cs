using System;
using System.Collections.Generic;
using System.IO;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.TextRepresentation;

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

            var content = ToLatex(document.Sections, 0);

            template = template.Replace("CONTENT", content);
            return template;
        }

        private static string ToLatex(List<Section> sections, int level)
        {
            var sectionName = "section";
            if (level == 1)
                sectionName = "sub" + sectionName;
            else
                sectionName = "subsub" + sectionName;
            var content = "";
            foreach (var documentSection in sections)
            {
                content += "\\" + sectionName + "{" + documentSection.Title + "}\n";
                content += ToLatex(documentSection.Paragraphs);
                content += ToLatex(documentSection.Sections, level + 1);
            }
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
                content = ToLatex(paragraph.LineNodes, "");
                content += "\\\\* "; //\\* 
            }
            return content;
        }

        private static string ToLatex(List<LineNode> lines, string prefix)
        {
            var content = "";
            foreach (var textNode in lines)
            {
                content += prefix + ToLatex(textNode) + "\n";
            }
            return content;
        }

        private static string ToLatex(LineNode line)
        {
            var content = ToLatex(line.TextNodes);
            content += "\\\\* "; //\\* 
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
            content += " \n";
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
            text = text.Replace("\\ ", "\\textbackslash ");
            text = text.Replace("~", "\\textasciitilde");
            text = text.Replace("^", "\\textasciicircum");
            var escapes = new[] {"&", "%", "$", "#", "_", "{", "}" };
            foreach (var escape in escapes)
            {
                text = text.Replace(escape, "\\" + escape);
            }
            return text;
        }
    }
}
