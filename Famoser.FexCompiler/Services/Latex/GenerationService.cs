using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Services.Interface;
using Famoser.FexCompiler.Services.Latex.Tree;

namespace Famoser.FexCompiler.Services.Latex
{
    public class GenerationService : IProcessService<string>
    {
        private readonly List<Section> _content;
        private readonly TextToLatexCompiler _textToLatexCompiler = new();

        private const int ParagraphOnParagraphSpacing = 5;

        public GenerationService(List<Section> content)
        {
            _content = content;
        }

        public string Process()
        {
            return _content
                .Aggregate("", (current, baseContent) => current + ToLatex(baseContent));
        }

        private string ToLatex(Section section, int level = 0)
        {
            var res = "";

            // print title
            var sectionName = "section";
            for (var i = 0; i < level && i < 4; i++) sectionName = "sub" + sectionName;
            var title = "\\" + sectionName + "{" + _textToLatexCompiler.Compile(section.Header.Text) + "}\n";
            res += title;

            // print content
            var content = ToLatex(section.Content);
            res += content;

            // if children have no children, output last level: paragraphs
            if (section.Children.All(c => !c.Children.Any()))
            {
                var paragraphSpacer = "\\vspace{" + ParagraphOnParagraphSpacing + "pt}\n";

                // if text before, add spacer now
                if (content != "")
                {
                    res += paragraphSpacer;
                }

                foreach (var sectionChild in section.Children)
                {
                    res += "\\textbf{" + _textToLatexCompiler.Compile(sectionChild.Header.Text) + "}\\\\\n";
                    res += ToLatex(sectionChild.Content);
                    res += paragraphSpacer;
                }
            }
            else
            {
                // else recurse
                foreach (var sectionChild in section.Children) res += ToLatex(sectionChild, level + 1);
            }

            return res;
        }

        private string ToLatex(List<Content> lines)
        {
            var content = "";
            foreach (var lineNode in lines)
                if (lineNode.ContentType == ContentType.Text)
                {
                    //write text
                    var textContent = _textToLatexCompiler.Compile(lineNode.Text);
                    if (textContent != "")
                        //latex newline + OS newline
                        content += textContent + "\\\\ " + Environment.NewLine;
                }
                else if (lineNode.ContentType == ContentType.Code)
                {
                    content += "\\begin{verbatim}\n" +
                               lineNode.Text +
                               "\n\\end{verbatim}";
                }

            return content;
        }
    }
}