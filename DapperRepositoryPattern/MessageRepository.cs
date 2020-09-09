using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DapperRepositoryPattern
{
    public class MessageRepository : GenericRepository<DbRecord>
    {
        public MessageRepository(string tableName) : base(tableName)
        { }

        protected override IEnumerable<PropertyInfo> GetProperties
        {
            get
            {
                return typeof(DbRecord)
                    .GetProperties()
                    .ToList()
                    //.Where(p => "message".Equals(p.Name)).ToArray();
                    .Where(p => p.GetCustomAttributes(typeof(SkipPropertyAttribute), true).Length == 0).ToArray();
            }
        }
    }
}
