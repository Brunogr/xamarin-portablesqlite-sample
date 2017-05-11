using Core.Data.Entity.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.Entity
{
    public class User : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}
