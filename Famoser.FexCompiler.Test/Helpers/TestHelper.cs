using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models;

namespace Famoser.FexCompiler.Test.Helpers
{
    class TestHelper
    {
        public static string GetInputFolderPath()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(basePath, "Input\\");
        }

        public static string[] GetInputFile(string fileName)
        {
            return File.ReadAllLines(GetInputFilePath(fileName));
        }

        public static string GetInputFilePath(string fileName)
        {
            return Path.Combine(GetInputFolderPath(), fileName);
        }

        public static ConfigModel GetConfigModel()
        {
            return new ConfigModel()
            {
                Author = "famoser",
                CompilePath = GetInputFolderPath()
            };
        }
    }
}
