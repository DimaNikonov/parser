using ParserLog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog
{
    class LogFileContext:DbContext
    {
        public LogFileContext()
            :base("DefaultConnection")
        {
        }

        public DbSet<LogInfo> logInfo { get; set; }
        public DbSet<InfoAboutFile> FilesInfo { get; set; }
        public DbSet<IpAddressCompany> IpAddressCompanis { get; set; }

    }
}
