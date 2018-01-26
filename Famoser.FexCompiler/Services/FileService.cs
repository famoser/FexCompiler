using System;
using System.Collections.Generic;
using System.IO;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Services.Interface;
using Newtonsoft.Json;

namespace Famoser.FexCompiler.Services
{
    public class FileService
    {
        public string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

        public void WriteFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public List<string> GetAllFexFilePaths(string basePath)
        {
            var res = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(basePath);

            foreach (FileInfo file in dir.GetFiles("*.fex"))
            {
                res.Add(file.FullName);
                Console.WriteLine("found {0}", file.Name);
            }
            foreach (var directoryInfo in dir.GetDirectories())
            {
                res.AddRange(GetAllFexFilePaths(directoryInfo.FullName));
            }
            return res;
        }
        
        private const string ConfigFileName = "config.json";
        public ConfigModel GetConfigModel()
        {
            //read out existing or create new config model
            string configFilePath = Path.Combine(PathHelper.GetAssemblyPath(), ConfigFileName);
            if (File.Exists(configFilePath))
            {
                var configFileContent = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<ConfigModel>(configFileContent);
            }
            return new ConfigModel();
        }

        public void SetConfigModel(ConfigModel configModel)
        {
            //save file
            string configFilePath = Path.Combine(PathHelper.GetAssemblyPath(), ConfigFileName);
            WriteFile(configFilePath, JsonConvert.SerializeObject(configModel));
        }
    }
}
