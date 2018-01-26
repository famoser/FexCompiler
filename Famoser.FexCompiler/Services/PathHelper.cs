using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Famoser.FexCompiler.Helpers
{
    public class PathHelper
    {
        public static string GetAssemblyPath(string path)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (location == null)
                throw new Exception("path not found häh");
            return Path.Combine(location, path);
        }

        public static List<string> GetAllFexFilePaths(string basePath)
        {
            var res = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(basePath);

            foreach (FileInfo file in dir.GetFiles("*.fex"))
            {
                res.Add(file.FullName);
                Console.WriteLine("found {0}", file.Name);
            }
            foreach (var directoryInfo in dir.GetDirectories())
            {
                res.AddRange(GetAllFexFilePaths(directoryInfo.FullName));
            }
            return res;
        }
    }
}
