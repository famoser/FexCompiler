using System.Collections.Generic;
using Famoser.FexCompiler.Models.Document;

namespace Famoser.FexCompiler.Models.LearningCard
{
    public class LearningCardCollection
    {
        public StatisticModel StatisticModel { get; set; }
        public MetaDataModel MetaDataModel { get; set; }
        public List<LearningCard> LearningCards { get; set; }
    }
}
