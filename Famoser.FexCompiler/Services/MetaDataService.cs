using System;
using System.IO;
using System.Security.Cryptography;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    public class MetaDataService : IProcessService<MetaDataModel>
    {
        private readonly ConfigModel _configModel;
        private readonly string _path;

        public MetaDataService(ConfigModel configModel, string path)
        {
            _configModel = configModel;
            _path = path;
        }

        public MetaDataModel Process()
        {
            var pathEntries = _path.Split(new[] { "\\" }, StringSplitOptions.None);
            var title = pathEntries[pathEntries.Length - 1];
            title = title.Substring(0, title.IndexOf(".", StringComparison.Ordinal));

            var changeDateTime = File.GetLastWriteTime(_path);

            //get file hash
            byte[] byteHash;
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(_path))
                {
                    byteHash = sha256.ComputeHash(stream);
                }
            }
            var hash = Convert.ToBase64String(byteHash);

            return new MetaDataModel
            {
                Author = _configModel.Author.Trim(),
                GeneratedAt = DateTime.Now,
                ChangedAt = changeDateTime,
                Title = title.Trim(),
                Hash = hash
            };
        }
    }
}
