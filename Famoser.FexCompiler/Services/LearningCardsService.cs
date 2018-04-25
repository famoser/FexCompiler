using System.Collections.Generic;
using System.Linq;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Models.Document.Content;
using Famoser.FexCompiler.Models.Document.TextRepresentation;
using Famoser.FexCompiler.Models.LearningCard;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    public class LearningCardsService : IProcessService<LearningCardCollection>
    {
        private readonly List<Section> _content;
        private readonly StatisticModel _statisticModel;
        private readonly MetaDataModel _metaDataModel;

        private const string PathSeparator = "→";

        public LearningCardsService(StatisticModel statisticModel, MetaDataModel metaDataModel, List<Section> content)
        {
            _statisticModel = statisticModel;
            _metaDataModel = metaDataModel;
            _content = content;
        }

        public LearningCardCollection Process()
        {
            var list = new List<LearningCard>();
            ToLearningCard(_content, list, "");
            return new LearningCardCollection()
            {
                LearningCards = list,
                MetaDataModel = _metaDataModel,
                StatisticModel = _statisticModel
            };
        }

        private void ToLearningCard(List<Section> sections, List<LearningCard> cards, string path)
        {
            if (!sections.Any()) return;

            foreach (var section in sections)
            {
                var header = LineToString(section.Header);

                //adapt paths for recursive cards
                var sectionPath = header;
                if (path.Length > 0)
                    sectionPath = path + " " + PathSeparator + " " + sectionPath;

                //create a card if section has text
                if (section.TextContent.Any())
                {
                    cards.Add(new LearningCard()
                    {
                        Title = header,
                        Content = LinesToString(section.TextContent),
                        ItemCount = section.TextContent.Count,
                        Path = path,
                        Identifier = sectionPath
                    });
                }
                //create card if no text, but children
                else if (section.Content.Count > 1)
                {
                    cards.Add(new LearningCard()
                    {
                        Title = header,
                        Content = ContentHeaderToString(section.Content),
                        ItemCount = section.Content.Count,
                        Path = path,
                        Identifier = sectionPath
                    });
                }


                //recursively include content
                ToLearningCard(section.Content, cards, sectionPath);

            }
        }

        private string LineToString(LineNode lineNode)
        {
            if (lineNode.TextNodes == null)
                return "";
            return lineNode.TextNodes.Aggregate("", (a, b) => a + b.Text);
        }

        private string LinesToString(List<LineNode> lineNode)
        {
            var res = "";
            foreach (var node in lineNode)
            {
                res += LineToString(node) + "\n";
            }
            res = res.Substring(0, res.Length - 1);
            return res;
        }

        private string ContentHeaderToString(List<Section> content)
        {
            var res = "";
            foreach (var section in content)
            {
                res += LineToString(section.Header) + "\n";
            }
            if (res.Length > 0)
                res = res.Substring(0, res.Length - 1);
            return res;
        }
    }
}
