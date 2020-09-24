using System;
using System.Collections.Concurrent;
using System.Linq;
using Sean.Core.SnowFlake.Contracts;

namespace Sean.Core.SnowFlake
{
    public class IdManager : IIdManager
    {
        /// <summary>
        /// ��������ID(0~31)
        /// </summary>
        public long DatacenterId { get; set; }
        /// <summary>
        /// ��������ID(0~31)
        /// </summary>
        public long WorkerId { get; set; }

        private static readonly ConcurrentDictionary<long, IdWorker> _instanceMap;

        /// <summary>
        /// �Ƿ��ʼ��������
        /// </summary>
        protected bool IsConfigInitialized { get; }

        static IdManager()
        {
            _instanceMap = new ConcurrentDictionary<long, IdWorker>();
        }

        public IdManager()
        {
            var enviVar = Environment.GetEnvironmentVariable("DatacenterAndWorkerId");// ��������ͨ����|���ָ�DatacenterId��WorkerId
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
        /// �����һ��ID (�̰߳�ȫ) 
        /// </summary>
        /// <returns>ȫ��Ψһ��id</returns>
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
