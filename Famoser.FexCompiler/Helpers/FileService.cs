using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models;

namespace Famoser.FexCompiler.Helpers
{
    public class FileService
    {
        private readonly string _path;
        public FileService(string path)
        {
            _path = path;
        }

        public DocumentModel Process()
        {
            var lines = File.ReadAllLines(_path);
            
            return new DocumentModel()
            {
                RawLines = lines,
                DocumentStats = new DocumentStats
                {
                    LineCount = lines.Length,
                    CharacterCount = lines.Sum(s => s.Length),
                    WordCount = lines.Sum(s1 => s1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                        .Count(s => s.Trim().Length > 0))
                }
            };
        }
    }
}
