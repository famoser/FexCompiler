using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            foreach (var path in paths)
            {
                var lines = File.ReadAllLines(path);
                var pathEntries = path.Split(new[] { "\\" }, StringSplitOptions.None);
                var title = pathEntries[pathEntries.Length - 1];

                var document = FexHelper.ParseDocument(lines.ToList(), title, config);
                var content = LatexHelper.CreateLatex(document);

                var texFile = path.Substring(0, path.LastIndexOf(".", StringComparison.Ordinal)) + ".tex";
                File.WriteAllText(texFile, content);
                var texPaths = texFile.Split(new[] { "\\" }, StringSplitOptions.None);

                Process p1 = new Process
                {
                    StartInfo =
                    {
                        FileName = "pdflatex",
                        Arguments = "\"" + texPaths[texPaths.Length - 1] + "\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                try
                {
                    Console.WriteLine("starting to compile for " + texPaths[texPaths.Length - 1]);
                    p1.Start();
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    Console.WriteLine("compiled latex for " + texPaths[texPaths.Length - 1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
