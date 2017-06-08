namespace Famoser.FexCompiler.Models
{
    public class ConfigModel
    {
        public string CompilePath { get; set; }
        public string Author { get; set; }

        public bool IsComplete()
        {
            return !(string.IsNullOrWhiteSpace(CompilePath) || string.IsNullOrWhiteSpace(Author));
        }
    }
}
