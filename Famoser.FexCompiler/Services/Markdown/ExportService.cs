using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services.Markdown
{
    public class ExportService : IProcessService<bool>
    {
        private readonly List<Section> _content;
        private readonly string _title;
        private readonly string _folder;
        private readonly string _fileNamePrefix;

        public ExportService(List<Section> content, string title, string folder, string fileNamePrefix)
        {
            _content = content;
            _title = title;
            _folder = folder;
            _fileNamePrefix = fileNamePrefix;
        }

        public bool Process()
        {
            var title = "# " + _title + "\n\n";
            var markdown = _content
                .Aggregate("", (current, baseContent) => current + ToMarkdown(baseContent));

            var path = _folder + Path.DirectorySeparatorChar + _fileNamePrefix + ".md";
            File.WriteAllText(path, title + markdown);

            return true;
        }

        private string ToMarkdown(Section section, int level = 1)
        {
            var result = new string('#', level + 1) + " " + section.Header.Text + "\n\n";
            
            var content = ToText(section.Content);
            if (!string.IsNullOrEmpty(content))
            {
                content += "\n";
            }

            result += content;

            // if children have no children, output last level: paragraphs
            if (section.Children.All(c => !c.Children.Any()))
            {
                foreach (var sectionChild in section.Children)
                {
                    result += sectionChild.Header.Text + "\n";
                    result += ToText(sectionChild.Content, "- ");
                    result += "\n";
                }
            }
            else
            {
                // else recurse
                foreach (var sectionChild in section.Children)
                {
                    result += ToMarkdown(sectionChild, level + 1);
                }
            }
            
            return result;
        }

        private string ToText(List<Content> lines, string textPrefix = "")
        {
            var content = "";

            foreach (var lineNode in lines.Where(node => !string.IsNullOrEmpty(node.Text)))
            {
                if (lineNode.ContentType == ContentType.Text)
                {
                    content += textPrefix + lineNode.Text + "\n";
                }
                else if (lineNode.ContentType == ContentType.Code)
                {
                    content += "```\n" + lineNode.Text + "```\n";
                }
            }


            return content;
        }
    }
}