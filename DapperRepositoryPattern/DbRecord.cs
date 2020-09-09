using System;
using System.Collections.Generic;
using System.Text;

namespace DapperRepositoryPattern
{
    public class DbRecord
    {
        //   public int Id { get; set; }
        public string message { get; set; }

        [SkipProperty]
        public DateTime CurrentTime { get; set; }

        [SkipProperty]
        public bool Visible { get; set; }
        public DbRecord(string _m)
        {
            message = _m;
            //    Visible = false;
        }
        public DbRecord(int id, string message)
        {
            //    Id = id;
            message = message;
        }
        public DbRecord(int id, string message, DateTime currentTime)
        {
            //    Id = id;
            message = message;
            //   CurrentTime = currentTime;
            //  Visible = true;
        }
    }
}
