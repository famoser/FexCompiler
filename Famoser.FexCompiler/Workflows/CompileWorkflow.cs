using System;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Workflows.Interface;

namespace Famoser.FexCompiler.Workflows
{
    public class CompileWorkflow : IWorkflow
    {
        private readonly FileService _fileService;
        private readonly ConfigModel _configModel;

        public CompileWorkflow(FileService fileService, ConfigModel configModel)
        {
            _fileService = fileService;
            _configModel = configModel;
        }

        public void DoWorkflow()
        {
            var paths = _fileService.GetAllFexFilePaths(_configModel.CompilePath);
            for (var index = 0; index < paths.Count; index++)
            {
                var path = paths[index];
                WorkflowStarted(path);

                //prepare meta data
                StepStarted("reading out meta data");
                var metaDataService = new MetaDataService(_configModel, path);
                var metaData = metaDataService.Process();
                StepCompleted();

                StepStarted("check properties of " + metaData.Title);
                var fexVersionService = new FexVersionService(metaData, path);
                StepCompleted();

                if (fexVersionService.NoChangesNeeded())
                {
                    WorkflowComplete(true, "no changes detected");
                    continue;
                }

                var document = new DocumentModel()
                {
                    MetaDataModel = metaData
                };

                //read out file
                StepStarted("read out lines");
                document.RawLines = _fileService.ReadFile(path);
                StepCompleted();

                //convert to fexLines
                StepStarted("parsing lines");
                var fexService = new FexService(document.RawLines);
                document.FexLines = fexService.Process();
                StepCompleted();

                //create statistic
                StepStarted("gathering statistics");
                var statisticService = new StatisticService(document.FexLines);
                document.StatisticModel = statisticService.Process();
                StepCompleted();

                //convert to content
                StepStarted("processing content");
                var contentService = new ContentService(document.FexLines);
                document.RootSection = contentService.Process();
                StepCompleted();

                //learning cards create
                StepStarted("creating learning cards");
                var learningCardsService = new LearningCardsService(document.StatisticModel, document.MetaDataModel,
                    document.RootSection.Content);
                var cards = learningCardsService.Process();
                StepCompleted();

                //learning cards persist
                StepStarted("persisting learning cards");
                var learningCardsExportService = new LearningCardsExportService(cards, path);
                var learningCardsFeedback = learningCardsExportService.Process();
                StepCompleted(learningCardsFeedback);

                //latex create
                StepStarted("creating latex");
                var latexService = new LatexService(document.StatisticModel, document.MetaDataModel, document.RootSection.Content);
                var latex = latexService.Process();
                StepCompleted();

                //latex compile
                StepStarted("compiling latex");
                var latexCompilerService = new LatexExportService(path, latex);
                var latexCompileFeedback = latexCompilerService.Process();
                StepCompleted(latexCompileFeedback);

                //output handout if requested
                if (_configModel.IncludeHandoutFormat)
                {
                    //recreate latex with new template name
                    StepStarted("creating handout latex");
                    latexService.SetTemplateName("Handout");
                    latex = latexService.Process();
                    StepCompleted();

                    //compile with new latex
                    StepStarted("compiling latex");
                    latexCompilerService.SetFilenameAppendix("_handout");
                    latexCompilerService.SetContent(latex);
                    latexCompileFeedback = latexCompilerService.Process();
                    StepCompleted(latexCompileFeedback);
                }
               
                var successful = latexCompileFeedback && learningCardsFeedback;


                if (successful)
                {
                    StepStarted("saving version information");
                    fexVersionService.MarkFexCompileSuccessful();
                    StepCompleted();
                }

                WorkflowComplete(successful);
            }
        }

        private void WorkflowStarted(string name)
        {
            _activeStep = 0;
            Console.WriteLine("started for " + name);
        }

        private int _activeStep;
        private void StepStarted(string name)
        {
            Console.Write("[" + _activeStep++ + "] " + name + "...");
        }

        private void StepCompleted(bool successful = true)
        {
            Console.WriteLine(" [" + (successful ? "successful" : "failed") + "]");
        }

        private void WorkflowComplete(bool successful, string customMessage = null)
        {
            string message;
            if (customMessage != null)
                message = customMessage;
            else
                message = successful ? "completed successfully" : "something went wrong :(";

            Console.WriteLine(message);
            Console.WriteLine();
        }
    }
}