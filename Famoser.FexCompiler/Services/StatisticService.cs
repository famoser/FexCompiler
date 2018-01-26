using System;
using System.Linq;
using Famoser.FexCompiler.Models.Document;

namespace Famoser.FexCompiler.Services
{
    public class StatisticService
    {
        private readonly string[] _lines;
        public StatisticService(string[] lines)
        {
            _lines = lines;
        }

        public StatisticModel Process()
        {
            return new StatisticModel
            {
                LineCount = _lines.Length,
                CharacterCount = _lines.Sum(s => s.Length),
                WordCount = _lines.Sum(s1 => s1.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Count(s => s.Trim().Length > 0))
            };
        }
    }
}
