using System;
using ParserLog.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog
{
    public class Writer<Tentity> : IGenericRepository<Tentity> where Tentity : class
    {
        DbContext _context;
        DbSet<Tentity> _dbSet;

        public Writer(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<Tentity>();
        }

        public async void WriteToDb(Tentity tentity)
        {
            _dbSet.Add(tentity);
            _context.SaveChanges();
        }

        
    }
}
