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
                var lines = File.ReadAllLines(path);
                var pathEntries = path.Split(new[] {"\\"}, StringSplitOptions.None);
                var title = pathEntries[pathEntries.Length - 1];

                var document = FexHelper.ParseDocument(lines.ToList(), title.Substring(0, title.Length - 4), config);
                TextHelper.Improve(document);

                var content = LatexHelper.CreateLatex(document);

                var baseFileName = path.Substring(0, path.LastIndexOf(".", StringComparison.Ordinal));
                var texFile = baseFileName + ".tex";
                File.WriteAllText(texFile, content);
                var texFolder = texFile.Substring(0, texFile.LastIndexOf("\\", StringComparison.Ordinal));


                var batFileLines = new string[2];
                batFileLines[0] = "del \"" + baseFileName + ".aux\"";
                batFileLines[1] = "pdflatex \"" + texFile + "\" -output-directory=\"" + texFolder + "\"";
                var batFilename = "batch" + index + ".bat";
                File.WriteAllLines(batFilename, batFileLines);

                try
                {
                    Console.WriteLine("###############################################");
                    Console.WriteLine("###############################################");
                    Console.WriteLine("##### starting to compile for " + title + " ####");
                    Console.WriteLine("##### file at " + texFile + " ####");
                    Console.WriteLine("###############################################");
                    Console.WriteLine("###############################################");
                    Process p1 = new Process
                    {
                        StartInfo =
                        {
                            FileName = batFilename,
                            UseShellExecute = false
                        }
                    };

                    p1.Start();
                    p1.WaitForExit();
                    Console.WriteLine("###############################################");
                    Console.WriteLine("#### compiled for " + title + " ####");
                    Console.WriteLine("###############################################");

                    File.Delete(batFilename);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
