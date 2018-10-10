using System.IO;

namespace KMobileProcessor.Controller
{
    public class DirectoryHelper
    {
        /// <summary>
        /// Get and return all directories from a specific folder path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetDirectories(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo ParentDirectory = new DirectoryInfo(path);
                DirectoryInfo[] Dicts = ParentDirectory.GetDirectories();
                return Dicts;
            }

            return null;
        }

        public static void CopyFiles(string sourceFolder, string targetFolder)
        {
            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                string fName = file.Substring(sourceFolder.Length + 1);
                File.Copy(Path.Combine(sourceFolder, fName), Path.Combine(targetFolder, fName));
            }
        }

        public static bool IsInDirectoryList(string dict, DirectoryInfo[] dictList)
        {
            bool inDirectory = false;

            foreach (DirectoryInfo directFile in dictList)
            {
                if (dict == directFile.Name)
                {
                    inDirectory = true;
                    break;
                }
            }

            return inDirectory;
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
