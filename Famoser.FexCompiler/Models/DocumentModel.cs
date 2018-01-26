using System.Collections.Generic;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.Content.Base;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models
{
    public class DocumentModel
    {
        public Section RootSection { get; set; }
        public string[] RawLines { get; set; }
        public List<FexLine> FexLines { get; set; }
        public StatisticModel StatisticModel { get; set; }
        public MetaDataModel MetaDataModel { get; set; }
    }
}
