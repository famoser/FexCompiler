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
        private readonly string _folder;
        private readonly string _filenamePrefix;

        public FexVersionService(MetaDataModel metaDataModel, string folder, string filenamePrefix)
        {
            _metaDataModel = metaDataModel;
            _folder = folder;
            _filenamePrefix = filenamePrefix;
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
            return _folder + Path.DirectorySeparatorChar + _filenamePrefix + ".version.json";
        }

        private string GetProgramVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }

            var version = new Version(2, 0, 0, 0);
            return version.ToString();
        }
    }
}
