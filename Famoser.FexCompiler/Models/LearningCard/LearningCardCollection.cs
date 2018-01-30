using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models.Document;

namespace Famoser.FexCompiler.Models.Export
{
    public class LearningCardCollection
    {
        public StatisticModel StatisticModel { get; set; }
        public MetaDataModel MetaDataModel { get; set; }
        public List<LearningCard> LearningCards { get; set; }
    }
}
