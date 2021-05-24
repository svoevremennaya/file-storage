using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace file_storage
{
    public static class FileStorage
    {
        public static string storage = @"C:\FileStorage";

        public static List<ItemInfo> GetFilesInCatalog(string path)
        {
            List<ItemInfo> catalog = new List<ItemInfo>();
            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            foreach (string item in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                catalog.Add(new ItemInfo("Directory", directoryInfo.Name));
            }
            foreach (string item in files)
            {
                FileInfo fileInfo = new FileInfo(item);
                catalog.Add(new ItemInfo("File", fileInfo.Name));
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

        public static string GetFullPath(string path)
        {
            if (path == null)
            {
                return storage;
            }
            else
            {
                return Path.Combine(storage, path);
            }
        }
    }
}
