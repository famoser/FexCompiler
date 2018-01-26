using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Famoser.FexCompiler.Helpers;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Services;
using Newtonsoft.Json;

namespace Famoser.FexCompiler
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to FexCompiler");

            var fileService = new FileService();
            //get config file
            var configModel = fileService.GetConfigModel();
            if (!configModel.IsComplete())
            {
                RenewConfiguration(configModel, fileService);
            }

            //worker loop
            while (true)
            {
                Console.WriteLine("Press (Enter) to compile with the existing configuration, (q) to quit or (c) to create new configuration");

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    //compile
                    Compile(configModel, fileService);
                }
                else if (key.Key == ConsoleKey.Q)
                {
                    //quit
                    break;
                }
                else if (key.Key == ConsoleKey.C)
                {
                    //create config
                    RenewConfiguration(configModel, fileService);
                }
                else
                {
                    Console.WriteLine("Operation not supported");
                }
            }
        }

        private static void RenewConfiguration(ConfigModel configModel, FileService fileService)
        {
            //configure
            Console.WriteLine("Choose path to look for .fex files");
            configModel.CompilePath = Console.ReadLine();
            Console.WriteLine("Choose your Author");
            configModel.Author = Console.ReadLine();

            //persist
            fileService.SetConfigModel(configModel);
        }

        private static void Compile(ConfigModel config, FileService fileService)
        {
            var paths = fileService.GetAllFexFilePaths(config.CompilePath);
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
                document.RawLines = fileService.ReadFile(path);
                Console.WriteLine("#");

                //convert to fexLines
                var fexService = new FexService(document.RawLines);
                document.FexLines = fexService.Process();
                Console.Write("#");

                //create statistic
                var statisticService = new StatisticService(document.FexLines);
                document.StatisticModel = statisticService.Process();
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

                //learning cards compile
                var learningCardsService = new LearningCardsService(document.RootSection.Content);
                var cards = learningCardsService.Process();

                //persist learning cards as json
                var baseFileName = path.Substring(0, path.LastIndexOf(".", StringComparison.Ordinal));
                var jsonFile = baseFileName + ".json";
                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(cards));
                //persist cards as csv
                var csvFile = baseFileName + ".csv";
                File.WriteAllText(csvFile, cards.Aggregate("", (a, b) => a + "\n\n\n\n" + b.Title + "\t" + b.Content).Substring(4));
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine();
            }
        }

    }
}
