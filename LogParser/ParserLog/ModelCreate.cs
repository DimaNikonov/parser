using ParserLog.Interface;
using ParserLog.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ParserLog
{
    public class ModelsCreate<Loginfo> : ICreate<Loginfo> where Loginfo : class
    {
        public const string site = "tariscope.com";
        LogInfo logInfo;
        InfoAboutFile infoAboutFile;
        IpAddressCompany ipAddressCompany;
        Writer<LogInfo> writerLogInfo;
        Writer<IpAddressCompany> writerIpAddress;
        
        public ModelsCreate(LogInfo logInfo, Writer<LogInfo> writerLogInfo,
            Writer<IpAddressCompany> writerIpAddress, IpAddressCompany ipAddressCompany, InfoAboutFile infoAboutFile)
        {
            this.infoAboutFile = infoAboutFile;
            this.ipAddressCompany = ipAddressCompany;
            this.logInfo = logInfo;
            this.writerLogInfo = writerLogInfo;
            this.writerIpAddress = writerIpAddress;
        }
        public void  CreatModel(string path)
        {
            Reader reader = new Reader();
            var listTemp = reader.ReadFromFile(path);
            foreach (var item in listTemp)
            {
                logInfo = FillModel(item, logInfo);
                writerLogInfo.WriteToDb(logInfo);
            }

            List<string> listIP = new List<string>();
            List<TempInfoFiles> listFileName = new List<TempInfoFiles>();
            using (LogFileContext db = new LogFileContext())
            {
                listIP = db.logInfo.Select(x => x.IpAddress).Distinct().ToList();
            }
            Task task = GetOwnersIpsAsync(listIP, writerIpAddress, ipAddressCompany);
            Task t = RunTasksAsync(infoAboutFile);
            task.Wait();
            t.Wait();

        }

        private async Task GetOwnersIpsAsync(List<string> list, Writer<IpAddressCompany> writerIpAddress, IpAddressCompany ipAddressCompany)
        {
            Stopwatch sw = Stopwatch.StartNew();

            foreach (var item in list)
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(string.Format("http://ip-api.com/xml/{0}?fields=org", item));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                writerIpAddress.WriteToDb(GetOwner(content, item, ipAddressCompany));
                Thread.Sleep(400);
            }
            sw.Stop();
            Console.WriteLine("record IpAddress is done");
        }

        private IpAddressCompany GetOwner(string content, string item, IpAddressCompany ipAddressCompany)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);
            var orgName = xmlDocument.InnerText;
            ipAddressCompany.IpAddress = item;
            ipAddressCompany.Company = orgName;
            return ipAddressCompany;
        }

        private LogInfo FillModel(string item, LogInfo logInfo)
        {
            var temp = item.Split(new string[] { " - - " }, StringSplitOptions.RemoveEmptyEntries);
            logInfo.IpAddress = temp[0];
            temp = temp[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            logInfo.Date = DateTime.ParseExact(temp[0], "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture);
            temp = temp[1].Split(new char[] { '"', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            logInfo.HttpRequestMethod = temp[0];
            logInfo.Path = temp[1];
            logInfo.HttpStatusCode = int.Parse(temp[3]);
            logInfo.Size = int.Parse(temp[4]);
            return logInfo;
        }

        private async Task RunTasksAsync(InfoAboutFile infoAboutFile)
        {
            List<TempInfoFiles> list;
            using (LogFileContext context = new LogFileContext())
            {
                list = context.logInfo.Select(x => new TempInfoFiles { PathName = x.Path, Size = x.Size }).ToList();
            }
            int countTread = 2;
            Task task0 = GetTitleAsync(list, 0, 1 * list.Count / countTread);
            Task task1 = GetTitleAsync(list, 1 * list.Count / countTread, 2 * list.Count / countTread);

            await Task.WhenAll(task0, task1);            
        }

        private async Task GetTitleAsync(List<TempInfoFiles> list, int start, int end)
        {
            await Task.Run(() =>
            {
                int count = 0;
                using (LogFileContext context = new LogFileContext())
                {
                    Writer<InfoAboutFile> writerInfoFile = new Writer<InfoAboutFile>(context);

                    Stream stream = null;
                    Encoding encoding = null;
                    string title = null;

                    for (int i = start; i < end; i++)
                    {
                        CreateStream(list, ref stream, ref encoding, i);
                        try
                        {
                            using (StreamReader reader = new StreamReader(stream, encoding))
                            {

                                while (reader.EndOfStream != true)
                                {
                                    title = reader.ReadLine();
                                    if (title.Contains("title"))
                                    {
                                        title = title.Split(new[] { "<title>", "</title>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                        break;
                                    }
                                }
                            }
                            context.FilesInfo.Add(ModelInfoAboutFileFill(list[i], title));
                            //writerInfoFile.WriteToDb(ModelInfoAboutFileFill(list[i], title));
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            context.FilesInfo.Add(ModelInfoAboutFileFill(list[i], string.Empty));
                            //writerInfoFile.WriteToDb(ModelInfoAboutFileFill(list[i], string.Empty));
                        }

                        catch (Exception e)
                        {
                            context.FilesInfo.Add(ModelInfoAboutFileFill(list[i], string.Empty));
                            //writerInfoFile.WriteToDb(ModelInfoAboutFileFill(list[i], string.Empty));
                        }
                        Console.WriteLine(count++);
                    }
                    context.SaveChangesAsync();
                }
            });
        }

        private InfoAboutFile ModelInfoAboutFileFill(TempInfoFiles item, string title)
        {
            InfoAboutFile infoAboutFile = new InfoAboutFile()
            {
                PathName = item.PathName,
                Size = item.Size,
                Title = title
            };
            return infoAboutFile;
        }

        private void CreateStream(List<TempInfoFiles> list, ref Stream stream, ref Encoding encoding, int i)
        {
            Uri uri = new Uri(string.Format($"http://tariscope.com{list[i].PathName}"));
            var request = WebRequest.Create(uri);

            WebResponse response = request.GetResponse();
            stream = response.GetResponseStream();
            encoding = Encoding.GetEncoding("utf-8");
        }
    }
}
