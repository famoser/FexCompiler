using System.IO;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services.Latex
{
    public class TemplateService : IProcessService<string>
    {
        private readonly string _latexContent;
        private readonly StatisticModel _statisticModel;
        private readonly MetaDataModel _metaDataModel;
        private readonly string _templateName;

        public const string DefaultTemplate = "Summary";
        public const string HandoutTemplate = "Handout";

        public TemplateService(string latexContent, StatisticModel statisticModel, MetaDataModel metaDataModel, string templateName)
        {
            _latexContent = latexContent;
            _statisticModel = statisticModel;
            _metaDataModel = metaDataModel;
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

            return template.Replace("CONTENT", _latexContent);
        }
    }
}