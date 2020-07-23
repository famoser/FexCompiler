using System.Collections.Generic;
using System.Linq;
using Famoser.FexCompiler.Models.Document;
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
                var header = section.Header.Text;

                //adapt paths for recursive cards
                var sectionPath = header;
                if (path.Length > 0)
                    sectionPath = path + " " + PathSeparator + " " + sectionPath;

                //create a card if section has text
                if (section.Content.Any())
                {
                    cards.Add(new LearningCard()
                    {
                        Title = header,
                        Content = LinesToString(section.Content),
                        ItemCount = section.Content.Count,
                        Path = path,
                        Identifier = sectionPath
                    });
                }
                //create card if no text, but children
                else if (section.Children.Count > 1)
                {
                    cards.Add(new LearningCard()
                    {
                        Title = header,
                        Content = ContentHeaderToString(section.Children),
                        ItemCount = section.Children.Count,
                        Path = path,
                        Identifier = sectionPath
                    });
                }


                //recursively include content
                ToLearningCard(section.Children, cards, sectionPath);

            }
        }

        private string LinesToString(List<Content> lineNode)
        {
            var res = "";
            foreach (var node in lineNode)
            {
                res += node.Text + "\n";
            }
            res = res.Substring(0, res.Length - 1);
            return res;
        }

        private string ContentHeaderToString(List<Section> content)
        {
            var res = "";
            foreach (var section in content)
            {
                res += section.Header.Text + "\n";
            }
            if (res.Length > 0)
                res = res.Substring(0, res.Length - 1);
            return res;
        }
    }
}
