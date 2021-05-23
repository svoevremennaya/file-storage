using System;
using System.Collections.Generic;
using System.IO;

namespace file_storage
{
    public static class FileStorage
    {
        public static string storage = @"C:\FileStorage";

        public static List<string> GetFilesInCatalog(string path)
        {
            List<string> catalog = new List<string>();
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                catalog.Add(fileInfo.Name);
            }

            return catalog;
        }

        public static int CopyFile(string srcPath, string destPath)
        {
            srcPath = Path.Combine(storage, srcPath);

            if (!File.Exists(srcPath))
            {
                return 404;
            }

            if (srcPath != destPath)
            {
                FileStream srcFile = new FileStream(srcPath, FileMode.Open);
                FileStream destFile = new FileStream(destPath, FileMode.Create);

                srcFile.CopyTo(destFile);

                srcFile.Close();
                destFile.Close();
            }

            return 200;
        }
    }
}
