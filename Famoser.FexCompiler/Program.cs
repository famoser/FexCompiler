using System;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Workflows;

namespace Famoser.FexCompiler
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to FexCompiler");

            var fileService = new FileService();
            var configModel = fileService.GetConfigModel();
            var compileWorkflow = new CompileWorkflow(fileService, configModel);

            var configWorkflow = new ConfigurationWorkflow(fileService, configModel);

            //get config file
            if (!configModel.IsComplete())
            {
                configWorkflow.DoWorkflow();
            }

            //worker loop
            while (true)
            {
                Console.WriteLine("Press (Enter) to compile with the existing configuration, (q) to quit or (c) to create new configuration");

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    compileWorkflow.DoWorkflow();
                }
                else if (key.Key == ConsoleKey.C)
                {
                    //create config
                    configWorkflow.DoWorkflow();
                }
                else if (key.Key == ConsoleKey.Q)
                {
                    //quit
                    break;
                }
                else
                {
                    Console.WriteLine("Operation not supported");
                }
            }
        }
    }
}
