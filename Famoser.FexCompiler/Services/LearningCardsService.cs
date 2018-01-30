﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.Content.Base;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Models.Export;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Services.Interface;
using Newtonsoft.Json;

namespace Famoser.FexCompiler.Services
{
    public class LearningCardsService : IProcessService<LearningCardCollection>
    {
        private readonly List<BaseContent> _content;
        private readonly StatisticModel _statisticModel;
        private readonly MetaDataModel _metaDataModel;

        private const string PathSeparator = "→";

        public LearningCardsService(StatisticModel statisticModel, MetaDataModel metaDataModel, List<BaseContent> content)
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

        private void ToLearningCard(List<BaseContent> baseContents, List<LearningCard> cards, string path)
        {
            if (baseContents.Any())
            {
                foreach (var baseContent in baseContents)
                {
                    if (baseContent is Section)
                    {
                        var section = (Section)baseContent;
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

                        //recursively include content
                        ToLearningCard(section.Content, cards, sectionPath);
                    }
                }
            }
        }

        private string LineToString(LineNode lineNode)
        {
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
    }
}
