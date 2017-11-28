using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog.Interface
{
    public interface ICreate<Tentity> where Tentity:class
    {
         void CreatModel(string path);
    }
}
