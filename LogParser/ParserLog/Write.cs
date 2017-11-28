using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog
{
    class Write
    {
        LogInfo logInfo = new LogInfo();

        private List<LogInfo> CreatetModelList(string pathToLogFile)
        {
            List<LogInfo> listLogInfo = new List<LogInfo>();

            Reader reader = new Reader();

            var listTemp = reader.ReadFromFile(pathToLogFile);
            Console.WriteLine(listTemp.Count);
            int count = 1;
            foreach (var item in listTemp)
            {
                var temp = item.Split(new string[] { " - - " }, StringSplitOptions.RemoveEmptyEntries);
                logInfo.IpAdress = temp[0];
                temp = temp[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                logInfo.Date = DateTime.ParseExact(temp[0], "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture);
                temp = temp[1].Split(new char[] { '"', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                logInfo.HttpRequestMethod = temp[0];
                logInfo.Path = temp[1];
                logInfo.HttpStatusCode = int.Parse(temp[3]);
                logInfo.Size = int.Parse(temp[4]);
                listLogInfo.Add(logInfo);
                Console.WriteLine(count);
                count++;
            }
            return listLogInfo;
        }

        public void WriteToDataBase(string pathToLogFile)
        {
            var modelList = CreatetModelList(pathToLogFile);
            using (LogFileContext context = new LogFileContext())
            {
                foreach (var item in modelList)
                {
                    context.logInfo.Add(item);
                }
                context.SaveChanges();
            }
        }

    }
}
