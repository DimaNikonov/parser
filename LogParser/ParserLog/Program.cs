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
using System.Xml.Serialization;

namespace ParserLog
{
    class Program
    {
        static void Main(string[] args)
        {
            const string pathToLogFile = @"../../LogIsHere/tariscope.access.log";

            List<string> ipList = new List<string>();
            List<string> fileNameList = new List<string>();
    
            LogInfo logInfo = new LogInfo();
            IpAddressCompany ipAddressCompany = new IpAddressCompany();
            InfoAboutFile infoAboutFile = new InfoAboutFile();
            Writer<LogInfo> writerLogInfo = new Writer<LogInfo>(new LogFileContext());
            Writer<IpAddressCompany> writerIpAddress = new Writer<IpAddressCompany>(new LogFileContext());
            ModelsCreate<LogInfo> create = new ModelsCreate<LogInfo>(logInfo, writerLogInfo, writerIpAddress, ipAddressCompany, infoAboutFile);
            create.CreatModel(pathToLogFile);            
        }        
    }
}

