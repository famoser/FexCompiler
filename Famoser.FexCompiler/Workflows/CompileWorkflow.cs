using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Services;
using Famoser.FexCompiler.Services.Latex;
using Famoser.FexCompiler.Services.LearningCards;
using Famoser.FexCompiler.Workflows.Interface;
using GenerationService = Famoser.FexCompiler.Services.LearningCards.GenerationService;

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
            var paths = _configModel.CompilePaths
                .Aggregate(new List<string>(),(current, path) =>
                {
                    current.AddRange(_fileService.GetAllFexFilePaths(path));
                    return current;
                });

            foreach (var path in paths)
            {
                WorkflowStarted(path);

                var result = CompileFile(path);
                if (!result.HasValue)
                {
                    WorkflowComplete(true, "No changes detected");
                }
                else
                {
                    WorkflowComplete(result.Value);
                }
            }
        }

        private bool? CompileFile(string path)
        {
            var successful = true;

            //get version information
            var metaDataService = new MetaDataService(_configModel, path);
            var metaData = metaDataService.Process();
            var fexVersionService = new FexVersionService(metaData, path);

            if (fexVersionService.NoChangesNeeded())
            {
                return null;
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

            (string folder, string filenamePrefix, string[] exportFormats) = ParsePath(path);

            if (exportFormats.Contains("json") || exportFormats.Contains("xlsx"))
            {
                //learning cards create
                StepStarted("creating learning cards");
                var learningCardsService = new GenerationService(document.StatisticModel, document.MetaDataModel,
                    document.RootSection.Children);
                var cards = learningCardsService.Process();
                StepCompleted();

                if (exportFormats.Contains("json"))
                {
                    //learning cards persist
                    StepStarted("persisting learning cards (json)");
                    var learningCardsExportService = new JsonExportService(cards, folder, filenamePrefix);
                    var learningCardsFeedback = learningCardsExportService.Process();
                    StepCompleted(learningCardsFeedback);

                    successful &= learningCardsFeedback;
                }

                if (exportFormats.Contains("xlsx"))
                {
                    //learning cards persist
                    StepStarted("persisting learning cards (xlsx)");
                    var learningCardsExportService = new XlsxExportService(cards, folder, filenamePrefix);
                    var learningCardsFeedback = learningCardsExportService.Process();
                    StepCompleted(learningCardsFeedback);

                    successful &= learningCardsFeedback;
                }
            }

            if (exportFormats.Contains("md"))
            {
                //latex create
                StepStarted("creating & storing markdown");
                var markdownService = new Services.Markdown.ExportService(document.RootSection.Children,
                    document.MetaDataModel.Title, folder, filenamePrefix);
                var markdownFeedback = markdownService.Process();
                StepCompleted();

                successful &= markdownFeedback;
            }

            if (exportFormats.Contains("pdf") || exportFormats.Contains("handout_pdf"))
            {
                //latex create
                StepStarted("creating latex");
                var latexService = new Services.Latex.GenerationService(document.RootSection.Children);
                var latex = latexService.Process();
                StepCompleted();

                if (exportFormats.Contains("pdf"))
                {
                    //latex template
                    StepStarted("preparing PDF template");
                    var latexTemplateService = new TemplateService(latex, document.StatisticModel, document.MetaDataModel,
                        TemplateService.DefaultTemplate);
                    var templatedLatex = latexTemplateService.Process();
                    StepCompleted();

                    //latex compile
                    StepStarted("compiling PDF latex");
                    var latexCompilerService = new CompilationService(templatedLatex, folder, filenamePrefix);
                    var latexCompileFeedback = latexCompilerService.Process();
                    StepCompleted(latexCompileFeedback);

                    successful &= latexCompileFeedback;
                }

                if (exportFormats.Contains("handout_pdf"))
                {
                    //latex template
                    StepStarted("preparing handout PDF template");
                    var latexTemplateService = new TemplateService(latex, document.StatisticModel, document.MetaDataModel,
                        TemplateService.HandoutTemplate);
                    var templatedLatex = latexTemplateService.Process();
                    StepCompleted();

                    //latex compile
                    StepStarted("compiling handout PDF latex");
                    var latexCompilerService = new CompilationService(templatedLatex, folder, filenamePrefix + "_handout");
                    var latexCompileFeedback = latexCompilerService.Process();
                    StepCompleted(latexCompileFeedback);

                    successful &= latexCompileFeedback;
                }
            }

            if (successful)
            {
                StepStarted("saving version information");
                fexVersionService.MarkFexCompileSuccessful();
                StepCompleted();
            }

            return successful;
        }

        private (string folder, string filenamePrefix, string[] exportFormats) ParsePath(string path)
        {
            var folder = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
            var filename = path.Substring(folder.Length + 1);

            var filenameComponents = filename.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (filenameComponents.Length == 2)
            {
                return (folder, filenameComponents[0], new[] {"pdf"});
            }

            var filenamePrefix = string.Join(".", filenameComponents.Take(filenameComponents.Length - 2));
            var exportFormats = filenameComponents[filenameComponents.Length - 2]
                    .Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

            return (folder, filenamePrefix, exportFormats);
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