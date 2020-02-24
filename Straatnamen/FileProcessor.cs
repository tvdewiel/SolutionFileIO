using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Straatnamen
{
    [Serializable]
    public class FileProcessor
    {
        private string path;
        private string extract;
        private string resultPath;
        private Dictionary<string, Dictionary<string, SortedSet<string>>> data;
        private class ProvincieGemeente
        {
            public string gemeenteNaam { get; set; }
            public string provincieNaam { get; set; }

            public ProvincieGemeente(string provincieNaam)
            {
                this.provincieNaam = provincieNaam;
            }
        }

        public FileProcessor(string path, string resultPath)
        {
            this.path = path;
            this.resultPath = resultPath;
            data = new Dictionary<string, Dictionary<string, SortedSet<string>>>();
        }
        public FileProcessor(string path, string extractpath, string resultPath)
        {
            this.path = path;
            this.extract = extractpath;
            this.resultPath = resultPath;
            data = new Dictionary<string, Dictionary<string, SortedSet<string>>>();
        }
        public void unZip(string filename, string subdir)
        {
            extract = subdir;
            ZipFile.ExtractToDirectory(Path.Combine(path, filename), Path.Combine(path, subdir));
        }
        public void readFiles(List<string> files)
        {
            Console.WriteLine("start reading files");
            //lees provincieIDs
            HashSet<int> provincieIds = new HashSet<int>();
            using (StreamReader p = new StreamReader(Path.Combine(path, extract, files[4])))
            {
                string[] ids = p.ReadLine().Trim().Split(",");
                foreach (string id in ids)
                {
                    provincieIds.Add(Int32.Parse(id));
                }
            }
            //lees provincienamen + provincieid + gemeenteid
            Dictionary<int, ProvincieGemeente> gemeenteProvincieLink = new Dictionary<int, ProvincieGemeente>();
            using (StreamReader gp = new StreamReader(Path.Combine(path, extract, files[3])))
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
            using (StreamReader g = new StreamReader(Path.Combine(path, extract, files[1])))
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
                    //else
                    //    Console.WriteLine($"{gemeenteID},{ss[3]} not found");
                }
            }
            //lees straatnaamid + gemeenteid
            Dictionary<int, int> straatnaamGemeenteLink = new Dictionary<int, int>();
            using (StreamReader sg = new StreamReader(Path.Combine(path, extract, files[2])))
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
                        //Console.WriteLine($"{gemeenteID},{straatnaamID} not found");
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
                        if (pg.gemeenteNaam != null)
                        {
                            if (data.ContainsKey(pg.provincieNaam)) //provincie bestaat 
                            {
                                if (data[pg.provincieNaam].ContainsKey(pg.gemeenteNaam)) //gemeente bestaat
                                {
                                    data[pg.provincieNaam][pg.gemeenteNaam].Add(ss[1]);
                                }
                                else //gemeente bestaat nog niet
                                {
                                    data[pg.provincieNaam].Add(pg.gemeenteNaam, new SortedSet<string> { ss[1] });
                                }
                            }
                            else //provincie bestaat nog niet
                            {
                                data.Add(pg.provincieNaam, new Dictionary<string, SortedSet<string>> { { pg.gemeenteNaam, new SortedSet<string>() { ss[1] } } });
                            }
                        }
                    }
                }
            }
            Console.WriteLine("end reading files");
        }
        private void schrijfGemeente(string path, SortedSet<string> straatnamen)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (string straatnaam in straatnamen)
                {
                    sw.WriteLine(straatnaam);
                }
            }
        }
        public void writeResults()
        {
            DirectoryInfo di = new DirectoryInfo(path);
            di.CreateSubdirectory(resultPath);

            foreach (string provincie in data.Keys)
            {
                string p = Path.Combine(resultPath, provincie);
                di.CreateSubdirectory(Path.Combine(resultPath, provincie));
                foreach (string gemeente in data[provincie].Keys)
                {
                    Console.WriteLine($"writing {provincie},{gemeente}");
                    schrijfGemeente(Path.Combine(path, resultPath, provincie, gemeente + ".txt"), data[provincie][gemeente]);
                }
            }
        }
        public void clearResultsFolder()
        {
            clearFolder(Path.Combine(path, resultPath));
        }
        private void clearFolder(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo fi in dir.GetFiles())
            {
                Console.WriteLine($"deleting {fi.FullName}");
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                Console.WriteLine($"deleting folder {di.FullName}");
            }
            dir.Delete();
        }
        public void writeClass()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"C:\NET\data\MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }
        public static FileProcessor readClass()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"C:\NET\data\MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            FileProcessor obj = (FileProcessor)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }
        public void writeFile()
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(path, "adresInfo.txt")))
            {
                foreach (var p in data.Keys)
                {
                    foreach (var y in data[p].Keys)
                    {
                        foreach (var z in data[p][y])
                        {
                            sw.WriteLine($"{p},{y},{z}");
                        }
                    }
                }
            }
        }
    }
}
