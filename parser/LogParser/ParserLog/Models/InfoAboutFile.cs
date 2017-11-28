using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog.Models
{
    public class InfoAboutFile
    {
        
        public int Id { get; set; }
        public string PathName { get; set; }
        public string Title { get; set; }
        public int Size { get; set; }
    }
}
