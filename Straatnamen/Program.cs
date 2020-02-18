using System;
using System.Collections.Generic;

namespace Straatnamen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //FileProcessor fp = new FileProcessor(@"c:\data", "straatnamen");
            FileProcessor fp = new FileProcessor(@"c:\data","extract","straatnamen");
            //fp.unZip("DirFileOefening.zip", "extract");
            fp.readFiles(new List<string>() { "WRstraatnamen.csv", "WRGemeentenaam.csv", "StraatnaamID_gemeenteID.csv", "ProvincieInfo.csv", "ProvincieIDsVlaanderen.csv" });
            fp.writeResults();
            //fp.clearResultsFolder();
        }
    }
}
