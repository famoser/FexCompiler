using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.Document;

namespace Famoser.FexCompiler.Helpers
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
