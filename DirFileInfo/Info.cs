using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DirFileInfo
{
    public class Info
    {
        private string path;
        private DirectoryInfo di;

        public Info(string path)
        {
            this.path = path;
            di = new DirectoryInfo(path);            
        }
        public void ShowDirectoryInfo()
        {
            // Dump directory information.           
            Console.WriteLine("***** Directory Info *****");
            Console.WriteLine("FullName: {0}", di.FullName);
            Console.WriteLine("Name: {0}", di.Name);
            Console.WriteLine("Parent: {0}", di.Parent);
            Console.WriteLine("Creation: {0}", di.CreationTime);
            Console.WriteLine("Attributes: {0}", di.Attributes);
            Console.WriteLine("Root: {0}", di.Root);
            Console.WriteLine("**************************\n");
        }
        public void Show10BiggestFiles()
        {
            Console.WriteLine("************Biggest**************");
            //zoek 10 grootste c# files
            FileInfo[] cFiles = di.GetFiles("*.cs", SearchOption.AllDirectories);
            SortedList<long, List<FileInfo>> slf = new SortedList<long, List<FileInfo>>();
            foreach (var f in cFiles)
            {
                //Console.WriteLine($"{f.Name},{f.Length}");
                if (slf.ContainsKey(f.Length)) { slf[f.Length].Add(f); }
                else slf.Add(f.Length, new List<FileInfo>() { f });
            }

            int fileCounter = 0;
            for (int i = slf.Count - 1; i >= 0; i--)
            {
                foreach (var fi in slf.Values[i])
                {
                    Console.WriteLine($"{fi.Name},{fi.Length}");
                    fileCounter++;
                    if (fileCounter == 10) break;
                }
                if (fileCounter == 10) break;
            }
            Console.WriteLine("**************************");
        }
        public void DirCreate(string path,string subdir)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            string p = Path.Combine(path, subdir);
            if (Directory.Exists(p)) {
                Directory.Delete(p,true); //true -> ook subdirectories
            }
            di.CreateSubdirectory(subdir);
            FileInfo f = new FileInfo(Path.Combine(p, "testfile"));
            f.Create();
        }
    }
}
