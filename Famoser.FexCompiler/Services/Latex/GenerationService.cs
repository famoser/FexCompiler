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
        private readonly TextToLatexCompiler _textToLatexCompiler = new TextToLatexCompiler();
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
                foreach (var baseContent in _content) _renderedContent += ToLatex(baseContent);
            }

            template = template.Replace("CONTENT", _renderedContent);
            return template;
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