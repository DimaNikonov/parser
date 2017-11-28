using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog
{
    public interface IGenericRepository<Tentity> where Tentity : class
    {
        void WriteToDb(Tentity tentity);
    }
}
