using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Straatnamen
{
    public class FileProcessor
    {
        private string path;
        private string extract;
        private Dictionary<string, Dictionary<string, List<string>>> data;
        private class ProvincieGemeente {
            public string gemeenteNaam { get; set; }
            public string provincieNaam { get; set; }

            public ProvincieGemeente(string provincieNaam)
            {
                this.provincieNaam = provincieNaam;
            }
        }

        public FileProcessor(string path)
        {
            this.path = path;
            data = new Dictionary<string, Dictionary<string, List<string>>>();
        }
        public FileProcessor(string path,string extractpath)
        {
            this.path = path;
            this.extract = extractpath;
            data = new Dictionary<string, Dictionary<string, List<string>>>();
        }
        public void unZip(string filename,string subdir)
        {
            extract = subdir;
            ZipFile.ExtractToDirectory(Path.Combine(path,filename), Path.Combine(path,subdir));
        }
        public void readFiles(List<string> files)
        {
            Console.WriteLine("start reading files");
            //lees provincieIDs
            HashSet<int> provincieIds = new HashSet<int>();
            using(StreamReader p=new StreamReader(Path.Combine(path,extract,files[4])))
            {
                string[] ids= p.ReadLine().Trim().Split(",");
                foreach(string id in ids)
                {
                    provincieIds.Add(Int32.Parse(id));
                }
            }
            //lees provincienamen + provincieid + gemeenteid
            Dictionary<int, ProvincieGemeente> gemeenteProvincieLink = new Dictionary<int, ProvincieGemeente>();
            using (StreamReader gp = new StreamReader(Path.Combine(path,extract, files[3])))
            {
                string line;
                gp.ReadLine(); //skip header
                while ((line = gp.ReadLine()) != null)
                {
                    string[] ss = line.Trim().Split(";");
                    int gemeenteID = Int32.Parse(ss[0]);
                    if (!gemeenteProvincieLink.ContainsKey(gemeenteID))
                    {
                        if (ss[2] == "nl")
                        {
                            if (provincieIds.Contains(Int32.Parse(ss[1])))
                            {
                                gemeenteProvincieLink.Add(gemeenteID, new ProvincieGemeente(ss[3]));
                            }
                        }
                    }
                }
            }
            //lees gemeentenamen + gemeenteid
            using(StreamReader g=new StreamReader(Path.Combine(path,extract,files[1])))
            {
                string line;
                g.ReadLine(); //skip header
                while ((line = g.ReadLine()) != null)
                {
                    string[] ss = line.Trim().Split(";");
                    int gemeenteID = Int32.Parse(ss[1]);
                    if (gemeenteProvincieLink.ContainsKey(gemeenteID))
                    {
                        if (ss[2] == "nl")
                            gemeenteProvincieLink[gemeenteID].gemeenteNaam = ss[3];
                    }
                    else
                        Console.WriteLine($"{gemeenteID},{ss[3]} not found");
                }
            }
            //lees straatnaamid + gemeenteid
            Dictionary<int, int> straatnaamGemeenteLink = new Dictionary<int, int>();
            using(StreamReader sg=new StreamReader(Path.Combine(path, extract, files[2])))
            {
                string line;
                sg.ReadLine(); //skip header
                while ((line = sg.ReadLine()) != null)
                {
                    string[] ss = line.Trim().Split(";");
                    int gemeenteID = Int32.Parse(ss[1]);
                    int straatnaamID = Int32.Parse(ss[0]);
                    if (!gemeenteProvincieLink.ContainsKey(gemeenteID))
                    {
                        Console.WriteLine($"{gemeenteID},{straatnaamID} not found");
                    }
                    else
                    {
                        straatnaamGemeenteLink.Add(straatnaamID, gemeenteID);
                    }
                }
            }
            //lees straatnamen
            using (StreamReader s = new StreamReader(Path.Combine(path, extract, files[0])))
            {
                string line;
                s.ReadLine(); //skip header
                while ((line = s.ReadLine()) != null)
                {
                    string[] ss = line.Trim().Split(";");
                    int straatnaamID = Int32.Parse(ss[0]);
                    if (straatnaamGemeenteLink.ContainsKey(straatnaamID))
                    {
                        ProvincieGemeente pg = gemeenteProvincieLink[straatnaamGemeenteLink[straatnaamID]];
                        if (pg.gemeenteNaam != null) {
                            if (data.ContainsKey(pg.provincieNaam)) //provincie bestaat 
                        {
                                if (data[pg.provincieNaam].ContainsKey(pg.gemeenteNaam)) //gemeente bestaat
                                {
                                    data[pg.provincieNaam][pg.gemeenteNaam].Add(ss[1]);
                                }
                                else //gemeente bestaat nog niet
                                {
                                    data[pg.provincieNaam].Add(pg.gemeenteNaam, new List<string> { ss[1] });
                                }
                            }
                            else //provincie bestaat nog niet
                            {
                                data.Add(pg.provincieNaam, new Dictionary<string, List<string>> { { pg.gemeenteNaam, new List<string>() { ss[1] } } });
                            }
                        }
                    }
                }
            }
            Console.WriteLine("end reading files");
        }
    }
}
