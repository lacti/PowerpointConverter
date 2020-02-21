using Swan.Logging;
using System;
using System.IO;

namespace ConvertWin32Core
{
    class FileUtils
    {
        public static void CleanupFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                e.Warn("FileUtils", $"Cannot delete file[{file}] due to");
            }
        }

        public static void CleanupDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory);
                }
            }
            catch (Exception e)
            {
                e.Warn("FileUtils", $"Cannot delete directory[{directory}] due to");
            }
        }

        public static void CleanupFileOrDirectory(string maybeFile)
        {
            try
            {
                if ((File.GetAttributes(maybeFile) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.Delete(maybeFile, true);
                }
                else
                {
                    if (File.Exists(maybeFile))
                    {
                        File.Delete(maybeFile);
                    }
                }
            }
            catch (Exception e)
            {
                e.Warn("FileUtils", $"Cannot delete file[{maybeFile}] due to");
            }
        }
    }
}
