using System;
using System.Deployment.Application;
using System.IO;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.Document;
using Newtonsoft.Json;

namespace Famoser.FexCompiler.Services
{
    public class FexVersionService
    {
        private readonly MetaDataModel _metaDataModel;
        private readonly string _path;

        public FexVersionService(MetaDataModel metaDataModel, string path)
        {
            _metaDataModel = metaDataModel;
            _path = path;
        }

        public bool NoChangesNeeded()
        {
            //create .json file
            var file = GetFexVersionPath();

            if (File.Exists(file))
            {
                var cacheContent = File.ReadAllText(file);
                var cache = JsonConvert.DeserializeObject<FexVersionModel>(cacheContent);

                return cache.ProgramVersion == GetProgramVersion() &&
                       cache.LastSuccessfulCompileHash == _metaDataModel.Hash;
            }

            return false;
        }

        public void MarkFexCompileSuccessful()
        {
            var cache = new FexVersionModel()
            {
                ProgramVersion = GetProgramVersion(),
                LastSuccessfulCompileHash = _metaDataModel.Hash
            };
            File.WriteAllText(GetFexVersionPath(), JsonConvert.SerializeObject(cache));
        }

        private string GetFexVersionPath()
        {
            var baseFileName = _path.Substring(0, _path.LastIndexOf(".", StringComparison.Ordinal));
            var fexVersionFile = baseFileName + "_version.json";
            return fexVersionFile;
        }

        private string GetProgramVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            var version = new Version(1, 1, 1, 1);
            return version.ToString();
        }
    }
}
