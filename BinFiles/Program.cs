using System;
using System.IO;

namespace BinFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string path = @"d:\.NET\test.bin";
            FileInfo f = new FileInfo(path);
            using (BinaryWriter bw=new BinaryWriter(f.OpenWrite()))
            {
                for(int i=1;i<=10;i++)
                {
                    bw.Write(i);
                }
            }
            using(BinaryReader br=new BinaryReader(f.OpenRead()))
            {
                while(br.BaseStream.Position!=br.BaseStream.Length)
                {
                    int x = br.ReadInt32();
                    Console.WriteLine(x);
                }
            }
        }
    }
}
