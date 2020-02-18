using System;
using System.IO;

namespace CreateDirectories
{
    class Program
    {
        public static void clearFolder(DirectoryInfo dir)
        {
            foreach (FileInfo fi in dir.GetFiles())
            {
                Console.WriteLine($"deleting {fi.FullName}");
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di);
                Console.WriteLine($"deleting folder {di.FullName}");
                di.Delete();
            }
        }
        public static void clearFolder(string folder)
        {
            string[] dirs=Directory.GetDirectories(folder);

            foreach (string fi in Directory.GetFiles(folder))
            {
                Console.WriteLine($"deleting {fi}");
                File.Delete(fi);
            }

            foreach (string di in dirs)
            {
                clearFolder(di);
                Console.WriteLine($"deleting folder {di}");
                Directory.Delete(di);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");            
            string path = @"D:\.NET\test";
            //scenario 1
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.CreateSubdirectory("subdir1");
            di.CreateSubdirectory("subdir2");
            di.CreateSubdirectory(@"subdir1\subsub");

            DirectoryInfo[] dil = di.GetDirectories();
            foreach (var d in dil)
            {
                Console.WriteLine(d.FullName, d.Parent);
            }
            Console.WriteLine("clean up (press enter)");
            Console.ReadLine();
            clearFolder(di);
            di.Delete();
            //scenario 2
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "subdir1"));
            Directory.CreateDirectory(Path.Combine(path, "subdir2"));
            File.Create(Path.Combine(path, "test")).Close();
            File.CreateText(Path.Combine(path, @"subdir1\txtfile.csv")).Close();
            
            Console.WriteLine("clean up (press enter)");
            Console.ReadLine();
            clearFolder(path);
            Directory.Delete(path);
        }
    }
}
