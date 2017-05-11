using Core.Data.Entity;
using Core.Data.Entity.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Sqlite
{
    public class SqliteRepository<T> where T : IEntity, new()
    {
        SqliteDatabase<T> db = null;
        public SqliteRepository(SQLiteConnection conn)
        {
            db = new SqliteDatabase<T>(conn);
        }

        public T Get(int id)
        {
            return db.GetItem(id);
        }

        public IEnumerable<T> Get()
        {
            return db.GetItems();
        }

        //public async Task<IEnumerable<T>> GetAsync()
        //{
        //    return await db.GetItems();
        //}

        public IEnumerable<T> GetByFilters(Expression<Func<T, bool>> aFilters)
        {
            return db.GetItems(aFilters);
        }

        public int Save(T item)
        {
            return db.SaveItem(item);
        }

        public int Delete(int id)
        {
            return db.DeleteItem(id);
        }

        public int DeleteAll()
        {
            return db.DeleteAll();
        }
    }
}
