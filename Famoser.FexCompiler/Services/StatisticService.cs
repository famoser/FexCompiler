using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.Document;

namespace Famoser.FexCompiler.Services
{
    public class StatisticService
    {
        private readonly List<FexLine> _lines;
        public StatisticService(List<FexLine> lines)
        {
            _lines = lines;
        }

        public StatisticModel Process()
        {
            return new StatisticModel
            {
                LineCount = _lines.Count,
                CharacterCount = _lines.Sum(s => s.Text.Length),
                WordCount = _lines
                    .Sum(s1 => s1.Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Count(s => s.Trim().Length > 0))
            };
        }
    }
}
