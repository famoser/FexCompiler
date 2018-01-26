using System.IO;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
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
