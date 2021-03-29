using System.IO;
using Famoser.FexCompiler.Models.LearningCard;
using Famoser.FexCompiler.Services.Interface;
using Newtonsoft.Json;

namespace Famoser.FexCompiler.Services.LearningCards
{
    public class JsonExportService : IProcessService<bool>
    {
        private readonly LearningCardCollection _learningCardCollection;
        private readonly string _folder;
        private readonly string _fileNamePrefix;

        public JsonExportService(LearningCardCollection learningCardCollection, string folder, string fileNamePrefix)
        {
            _learningCardCollection = learningCardCollection;
            _folder = folder;
            _fileNamePrefix = fileNamePrefix;
        }

        public bool Process()
        {
            var json = JsonConvert.SerializeObject(_learningCardCollection);

            var path = _folder + Path.DirectorySeparatorChar + _fileNamePrefix + ".json";
            File.WriteAllText(path, json);

            return true;
        }
    }
}
