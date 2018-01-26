using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models;
using Newtonsoft.Json;

namespace Famoser.FexCompiler
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to FexCompiler");

            //get config file
            var configModel = GetConfigModel();
            if (configModel == null || !configModel.IsComplete())
            {
                configModel = CreateConfiguration();
            }

            //worker loop
            while (true)
            {
                Console.WriteLine("Press (Enter) to compile with the existing configuration, (q) to quit or (c) to create new configuration");

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    //compile
                    Compile(configModel);
                }
                else if (key.Key == ConsoleKey.Q)
                {
                    //quit
                    break;
                }
                else if (key.Key == ConsoleKey.C)
                {
                    //create config
                    configModel = CreateConfiguration();
                }
                else
                {
                    Console.WriteLine("Operation not supported");
                }
            }
        }

        private static ConfigModel CreateConfiguration()
        {
            var configModel = new ConfigModel();

            //configuration init
            Console.WriteLine("Choose path to look for .fex files");
            configModel.CompilePath = Console.ReadLine();
            Console.WriteLine("Choose your Author");
            configModel.Author = Console.ReadLine();

            //save file
            string configFilePath = PathHelper.GetAssemblyPath(ConfigFileName);
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(configModel));
            return configModel;
        }

        private static void Compile(ConfigModel config)
        {
            var paths = PathHelper.GetAllFexFilePaths(config.CompilePath);
            for (var index = 0; index < paths.Count; index++)
            {
                var path = paths[index];

                //prepare meta data
                var metaDataService = new MetaDataService(config, path);
                var metaData = metaDataService.Process();
                Console.WriteLine("compiling " + metaData.Title);

                //todo: check if file needs to be regenerated

                Console.Write("#");
                var document = new DocumentModel()
                {
                    MetaDataModel = metaData
                };

                //read out file
                var fileService = new FileService(path);
                document.RawLines = fileService.Process();
                Console.WriteLine("#");

                //create statistic
                var statisticService = new StatisticService(document.RawLines);
                document.StatisticModel = statisticService.Process();
                Console.Write("#");

                //convert to fexLines
                var fexService = new FexService(document.RawLines);
                document.FexLines = fexService.Process();
                Console.Write("#");

                //convert to content
                var contentService = new ContentService(document.FexLines);
                document.RootSection = contentService.Process();
                Console.Write("#");

                //latex output
                var latexService = new LatexService(document.StatisticModel, document.MetaDataModel, document.RootSection.Content);
                var latex = latexService.Process();
                Console.Write("#");

                //latex compile
                var latexCompilerService = new LatexCompilerService(path, latex);
                var latexCompileFeedback = latexCompilerService.Process();
                if (latexCompileFeedback)
                    Console.WriteLine(metaData.Title + " compiled successfully");
                else
                    Console.WriteLine(metaData.Title + " failed to compile");
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine();
            }
        }

        private static string ConfigFileName = "config.json";
        private static ConfigModel GetConfigModel()
        {
            //read out existing or create new config model
            string configFilePath = PathHelper.GetAssemblyPath(ConfigFileName);
            if (File.Exists(configFilePath))
            {
                var configFileContent = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<ConfigModel>(configFileContent);
            }
            return new ConfigModel();
        }
    }
}
