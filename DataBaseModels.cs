using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace english_wpf
{
    class RandomWordsContext : DbContext
    {
        public RandomWordsContext() // CreatingDB Code First
        {
            //  Database.EnsureCreated();
        }
        public DbSet<Word> Words { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlite("Data Source=" + DbNames.dbName  /* +*DbNames.dbName*/);
    }

    public class Word
    {
        public int WordId { get; set; }
        public string EnglishWord { get; set; }
        public string RussianWord { get; set; }
        public int Try { get; set; }
    }
}
