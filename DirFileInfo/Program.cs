using System;
using System.Collections.Generic;
using System.IO;

namespace DirFileInfo
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string path = @"c:\NET";
            Info info = new Info(path);
            info.ShowDirectoryInfo();
            info.Show10BiggestFiles();
            info.DirCreate(@"c:\NET\","test");
                       

        }
    }
}
