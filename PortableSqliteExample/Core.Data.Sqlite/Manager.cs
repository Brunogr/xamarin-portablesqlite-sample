using Core.Data.Entity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Sqlite
{
    public class SqliteManager
    {
        public SqliteRepository<User> UserRepository;

        public SqliteManager(SQLiteConnection conn)
        {
            UserRepository = new SqliteRepository<User>(conn);
        }
    }
}
