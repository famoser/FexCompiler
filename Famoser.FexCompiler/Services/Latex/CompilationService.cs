using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services.Latex
{
    public class CompilationService : IProcessService<bool>
    {
        private string _content;
        private readonly string _folder;
        private readonly string _fileNamePrefix;

        public CompilationService(string content, string folder, string fileNamePrefix)
        {
            _content = content;
            _folder = folder;
            _fileNamePrefix = fileNamePrefix;
        }

        public bool Process()
        {
            //create .tex file
            var path = _folder + Path.DirectorySeparatorChar + _fileNamePrefix;
            var texFile = path + ".tex";
            File.WriteAllText(texFile, _content);

            //clean up old compilations
            var auxFile = path + ".aux";
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

                TryRemove(path + ".log");
                TryRemove(path + ".tex");

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
