using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models.TextRepresentation.Base;

namespace Famoser.FexCompiler.Models.TextRepresentation
{
    public class Code : Content
    {
        public Code(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
