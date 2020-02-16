using System;
using System.IO;

namespace TextFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string path = @"d:\.NET\test.txt";
            using(StreamWriter sw=new StreamWriter(path,true)) //append use true
            {
                for(int i=1;i<11;i++)
                {
                    sw.WriteLine($"This is line {i} in the file");
                }
            }
            using(StreamReader sr=new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
