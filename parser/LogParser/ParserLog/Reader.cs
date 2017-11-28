using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog
{
    public class Reader
    {
        List<string> listRowLogFile = new List<string>();
        List<string> listContains = new List<string>()
        {
            "?download=", "media", "templates", "images","/administrator/index.php",
            "css", "gif", "txt", "js", "png", "jpg", "ico"
        };

        bool flag = false;

        public List<string> ReadFromFile(string pathToLogFile)
        {
            using (var sReader = new StreamReader(pathToLogFile))
            {
                string row;
                while ((row = sReader.ReadLine()) != null)
                {
                    foreach (var item in listContains)
                    {
                        if (flag = row.Contains(item))
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        continue;
                    }
                    listRowLogFile.Add(row);
                    flag = false;
                }
            }
            return listRowLogFile;
        }
    }
}
