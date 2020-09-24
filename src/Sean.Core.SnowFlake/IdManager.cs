using System;
using System.Collections.Concurrent;
using System.Linq;
using Sean.Core.SnowFlake.Contracts;

namespace Sean.Core.SnowFlake
{
    public class IdManager : IIdManager
    {
        /// <summary>
        /// 数据中心ID(0~31)
        /// </summary>
        public long DatacenterId { get; set; }
        /// <summary>
        /// 工作机器ID(0~31)
        /// </summary>
        public long WorkerId { get; set; }

        private static readonly ConcurrentDictionary<long, IdWorker> _instanceMap;

        /// <summary>
        /// 是否初始化过配置
        /// </summary>
        protected bool IsConfigInitialized { get; }

        static IdManager()
        {
            _instanceMap = new ConcurrentDictionary<long, IdWorker>();
        }

        public IdManager()
        {
            var enviVar = Environment.GetEnvironmentVariable("DatacenterAndWorkerId");// 环境变量通过“|”分隔DatacenterId和WorkerId
            if (!string.IsNullOrWhiteSpace(enviVar))
            {
                var idList = enviVar.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(c =>
                {
                    if (int.TryParse(c, out var result) && result >= 0)
                    {
                        return result;
                    }
                    return 0;
                }).ToList();
                if (idList.Any())
                {
                    IsConfigInitialized = true;
                    DatacenterId = Math.Min(idList[0], IdWorker.MaxDatacenterId);
                    if (idList.Count > 1)
                    {
                        WorkerId = Math.Min(idList[1], IdWorker.MaxWorkerId);
                    }
                }
            }
        }

        /// <summary>
        /// 获得下一个ID (线程安全) 
        /// </summary>
        /// <returns>全局唯一的id</returns>
        public virtual long NextId()
        {
            var idWorker = _instanceMap.GetOrAdd(DatacenterId, new IdWorker(DatacenterId, WorkerId));
            return idWorker.NextId();
        }
    }
    public class IdManager<T> : IdManager, IIdManager<T> where T : class
    {
        public IdManager()
        {
            if (DatacenterId == 0 && !IsConfigInitialized)
            {
                DatacenterId = Math.Abs(typeof(T).FullName.GetHashCode()) % 32;
            }
        }
    }
}
