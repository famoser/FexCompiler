using System.IO;
using System.Reflection;

namespace Famoser.FexCompiler.Helpers
{
    public class PathHelper
    {
        public static string GetAssemblyPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
