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
        public static string[] GetInputFile(string fileName)
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return File.ReadAllLines(Path.Combine(basePath, "Input/" + fileName));
        }

        public static ConfigModel GetConfigModel()
        {
            return new ConfigModel()
            {
                Author = "famoser",
                CompilePath = "C:/"
            };
        }
    }
}
