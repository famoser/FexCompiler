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
            Console.WriteLine("Choose path to look for .fex files");
            _configModel.CompilePath = Console.ReadLine();
            Console.WriteLine("Choose your Author");
            _configModel.Author = Console.ReadLine();

            //persist
            _fileService.SetConfigModel(_configModel);
        }
    }
}
