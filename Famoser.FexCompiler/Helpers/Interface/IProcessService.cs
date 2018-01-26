using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.FexCompiler.Helpers.Interface
{
    public interface IProcessService<T>
    {
        T Process();
    }
}
