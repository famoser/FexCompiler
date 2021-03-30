using System;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Workflows.Interface;

namespace Famoser.FexCompiler.Workflows
{
    class ConfigurationWorkflow : IWorkflow
    {
        private readonly FileService _fileService;
        private readonly ConfigModel _configModel;

        public ConfigurationWorkflow(FileService fileService, ConfigModel configModel)
        {
            _fileService = fileService;
            _configModel = configModel;
        }

        public void DoWorkflow()
        {
            //configure
            Console.WriteLine("Add path(s) to look for .fex files. Confirm with [Enter]. Confirm an empty line to continue.");
            _configModel.CompilePaths.Clear();
            while (true)
            {
                var path = Console.ReadLine();
                if (string.IsNullOrEmpty(path))
                {
                    break;
                }

                _configModel.CompilePaths.Add(path);
            }

            Console.WriteLine("Choose your Author.");
            _configModel.Author = Console.ReadLine();

            //persist
            _fileService.SetConfigModel(_configModel);

            Console.WriteLine("\nSetup finished.\n\n");
        }
    }
}
