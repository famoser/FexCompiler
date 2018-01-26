using System;
using System.Diagnostics;
using System.IO;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    public class LatexCompilerService : IProcessService<bool>
    {
        private readonly string _path;
        private readonly string _content;

        public LatexCompilerService(string path, string content)
        {
            _path = path;
            _content = content;
        }

        public bool Process()
        {
            //create .tex file
            var baseFileName = _path.Substring(0, _path.LastIndexOf(".", StringComparison.Ordinal));
            var texFile = baseFileName + ".tex";
            File.WriteAllText(texFile, _content);

            //clean up old compilations
            var auxFile = baseFileName + ".aux";
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
                        UseShellExecute = false
                    }
                };

                p1.Start();
                p1.WaitForExit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
