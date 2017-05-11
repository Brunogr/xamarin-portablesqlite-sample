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
    public class SqliteDatabase<T> where T : IEntity, new()
    {
        static object locker = new object();

        public SQLiteConnection database;

        public string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        public SqliteDatabase(SQLiteConnection conn)
        {
            database = conn;
            // create the tables
            database.CreateTable<T>();
        }

        public IEnumerable<T> GetItems()
        {
            lock (locker)
            {
                return (from i in database.Table<T>() select i).ToList();
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            lock (locker)
            {
                return (from i in database.Table<T>() select i).ToList();
            }
        }

        public IEnumerable<T> GetItems(Expression<Func<T, bool>> aFilters)
        {
            lock (locker)
            {
                return database.Table<T>().Where(aFilters).ToList();                
            }
        }

        public T GetItem(int id)
        {
            lock (locker)
            {
                return database.Table<T>().FirstOrDefault(x => x.Id == id);
                // Following throws NotSupportedException - thanks aliegeni
                //return (from i in Table<T> ()
                //        where i.ID == id
                //        select i).FirstOrDefault ();
            }
        }

        public int SaveItem(T item)
        {
            lock (locker)
            {
                if (item.Id != 0)
                {
                    database.Update(item);
                    return item.Id;
                }
                else
                {
                    return database.Insert(item);
                }
            }
        }

        public int DeleteItem(int id)
        {
            lock (locker)
            {
                return database.Delete<T>(id);
            }
        }

        public int DeleteAll()
        {
            lock (locker)
            {
                return database.DeleteAll<T>();
            }
        }
    }
}
