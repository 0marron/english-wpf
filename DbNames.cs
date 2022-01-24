using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace english_wpf
{
    class DbNames
    {
        public static string dbName;
        public DbNames(object cbName)
        {
            dbName = cbName as String;
        }
    }
}
