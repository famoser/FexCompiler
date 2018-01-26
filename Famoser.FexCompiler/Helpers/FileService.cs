using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Helpers.Interface;
using Famoser.FexCompiler.Models;

namespace Famoser.FexCompiler.Helpers
{
    public class FileService : IProcessService<string[]>
    {
        private readonly string _path;
        public FileService(string path)
        {
            _path = path;
        }

        public string[] Process()
        {
            return File.ReadAllLines(_path);
        }
    }
}
