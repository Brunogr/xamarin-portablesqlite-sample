using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Data.Sqlite;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using SQLite;
using System.IO;
using SQLite;

namespace PortableSqliteExample
{
    [Application]
    public class AppStartup : Application
    {
        public static AppStartup Current { get; private set; }
        public SqliteManager Manager { get; set; }
        //public static SqliteRepository<T> Repository { get; set; }
        SQLiteConnection conn;
        public AppStartup(IntPtr handle, global::Android.Runtime.JniHandleOwnership transfer)
			: base(handle, transfer) {
            Current = this;
        }
        public override void OnCreate()
        {
            base.OnCreate();

            var sqliteFilename = "PortableSqliteDB.db3";
            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(libraryPath, sqliteFilename);
            conn = new SQLiteConnection(path);

            Manager = new SqliteManager(conn);
        }
    }
}