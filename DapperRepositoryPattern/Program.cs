using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DapperRepositoryPattern
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var messageRepository = new MessageRepository("messages");
            var data = await messageRepository.GetAllAsync();
            List<DbRecord> messages = new List<DbRecord>();
            for (int i = 0; i < 50000; i++)
            {
                messages.Add(new DbRecord(DateTime.Now.ToString()));
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            await messageRepository.InsertAsync(new DbRecord(DateTime.Now.ToString()));
            // await messageRepository.SaveRangeAsync(messages);
            stopwatch.Stop();
            var elapsed_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Elapsed time {elapsed_time} ms");
        }
    }
}
