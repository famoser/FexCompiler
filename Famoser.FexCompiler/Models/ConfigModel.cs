using System.Collections.Generic;

namespace Famoser.FexCompiler.Models
{
    public class ConfigModel
    {
        public List<string> CompilePaths { get; set; } = new();
        public string Author { get; set; }

        public bool IsComplete()
        {
            return CompilePaths.Count > 0 && !string.IsNullOrWhiteSpace(Author);
        }
    }
}
