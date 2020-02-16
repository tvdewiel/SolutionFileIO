using System;
using System.Collections.Generic;
using System.IO;

namespace DirFileInfo
{
    class Program
    {
        static void ShowDirectoryInfo(DirectoryInfo dir)
        {
            // Dump directory information.           
            Console.WriteLine("***** Directory Info *****");
            Console.WriteLine("FullName: {0}", dir.FullName);
            Console.WriteLine("Name: {0}", dir.Name);
            Console.WriteLine("Parent: {0}", dir.Parent);
            Console.WriteLine("Creation: {0}", dir.CreationTime);
            Console.WriteLine("Attributes: {0}", dir.Attributes);
            Console.WriteLine("Root: {0}", dir.Root);
            Console.WriteLine("**************************\n");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string path = @"d:\.NET";

            DirectoryInfo di = new DirectoryInfo(path);
            ShowDirectoryInfo(di);
            
            //zoek 10 grootste c# files
            FileInfo[] cFiles = di.GetFiles("*.cs",SearchOption.AllDirectories);
            SortedList<long,List<FileInfo>> slf = new SortedList<long,List<FileInfo>>();
            foreach (var f in cFiles)
            {
                //Console.WriteLine($"{f.Name},{f.Length}");
                if (slf.ContainsKey(f.Length)) { slf[f.Length].Add(f); }
                else slf.Add(f.Length, new List<FileInfo>() { f });               
            }
    
            int fileCounter = 0;
            for(int i=slf.Count-1;i>=0 ;i--)
            {
                foreach (var fi in slf.Values[i])
                {
                    Console.WriteLine($"{fi.Name},{fi.Length}");
                    fileCounter++;
                    if (fileCounter == 10) break;
                }
                if (fileCounter == 10) break;
            }

        }
    }
}
