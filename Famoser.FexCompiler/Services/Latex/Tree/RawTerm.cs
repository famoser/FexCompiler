using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.FexCompiler.Services.Latex.Tree
{
    class RawTerm: Term
    {
        public RawTerm(string content)
        {
            Content = content;
        }

        public string Content { private set; get; }
    }
}
