using System;
using System.Diagnostics;
using Sean.Core.SnowFlake;
using Sean.Utility.Common;
using Sean.Utility.Enums;
using Sean.Utility.Extensions;

namespace Demo.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(DateTimeHelper.ToTimestamp(DateTime.Today));
            //Console.WriteLine(DateTimeHelper.ToDateTime(1600876800000L, TimestampType.JavaScript).ToLongDateTimeWithTimezone());

            var idManager = new IdManager<Program>();
            var count = 100;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            DelegateHelper.BatchFunc(() =>
            {
                var id = idManager.NextId();
                Console.WriteLine(id);
                return false;
            }, count);
            stopwatch.Stop();
            Console.WriteLine($"生成{count}个id，耗时：{stopwatch.ElapsedMilliseconds}ms");

            Console.ReadLine();
        }
    }
}
