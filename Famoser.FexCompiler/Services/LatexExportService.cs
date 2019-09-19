using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    public class LatexExportService : IProcessService<bool>
    {
        private readonly string _path;
        private string _content;
        private string _fileNameAppendix = "";

        public LatexExportService(string path, string content)
        {
            _path = path;
            _content = content;
        }

        public void SetFilenameAppendix(string appendix)
        {
            _fileNameAppendix = appendix;
        }

        public void SetContent(string content)
        {
            _content = content;
        }

        public bool Process()
        {
            //create .tex file
            var baseFileName = _path.Substring(0, _path.LastIndexOf(".", StringComparison.Ordinal));
            var newFilename = baseFileName + _fileNameAppendix;
            var texFile = newFilename + ".tex";
            File.WriteAllText(texFile, _content);

            //clean up old compilations
            var auxFile = newFilename + ".aux";
            TryRemove(auxFile);

            //create bat file with commands
            var folder = texFile.Substring(0, texFile.LastIndexOf("\\", StringComparison.Ordinal));
            var batFilename = "batch.bat";
            File.WriteAllText(batFilename, "pdflatex \"" + texFile + "\" -output-directory=\"" + folder + "\"");

            //invoke .bat
            try
            {
                Process p1 = new Process
                {
                    StartInfo =
                    {
                        FileName = batFilename,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    }
                };

                bool failed = false;
                p1.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data) && args.Data.StartsWith("! "))
                    {
                        failed = true;
                    }
                };

                p1.Start();
                p1.BeginOutputReadLine();
                p1.BeginErrorReadLine();

                while (true)
                {
                    if (failed)
                    {
                        p1.Kill();
                        break;
                    }

                    if (p1.HasExited)
                    {
                        break;
                    }

                    Thread.Sleep(200);
                }

                if (failed)
                {
                    return false;
                }

                TryRemove(newFilename + ".log");
                TryRemove(newFilename + ".tex");

                return true;
            }
            catch
            {
                //too bad
                return false;
            }
            finally
            {
                TryRemove(batFilename);
                TryRemove(auxFile);
            }
        }

        private void TryRemove(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch
            {
                //well no one can say we did not try
            }
        }
    }
}
