using System;

namespace Sean.Core.SnowFlake
{
    public class IdWorker
    {
        private const long Twepoch = 1600876800000L;//开始时间截（2020-09-24T00:00:00+08:00），可自定义，小于当前时间即可
        private const int DatacenterIdBits = 5;
        private const int WorkerIdBits = 5;
        private const int SequenceBits = 12;
        internal const long MaxDatacenterId = -1 ^ (-1L << DatacenterIdBits);
        internal const long MaxWorkerId = -1 ^ (-1L << WorkerIdBits);
        private const long SequenceMask = -1 ^ (-1L << SequenceBits);
        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        private readonly long _datacenterId;
        private readonly long _workerId;
        private long _sequence;

        private static long _lastTimestamp = -1L;

        private static readonly object Locker = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datacenterId">数据中心ID(0~31)</param>
        /// <param name="workerId">工作机器ID(0~31)</param>
        public IdWorker(long datacenterId, long workerId)
        {
            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(datacenterId), $"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
            }
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(workerId), $"worker Id can't be greater than {MaxWorkerId} or less than 0");
            }

            _datacenterId = datacenterId;
            _workerId = workerId;
        }

        /// <summary>
        /// 获得下一个ID (线程安全) 
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (Locker)
            {
                var timestamp = GetCurTimestamp();
                if (timestamp < _lastTimestamp)
                {
                    //如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过，需要抛出异常
                    throw new InvalidSystemClockException($"Clock moved backwards, Refusing to generate id for {_lastTimestamp - timestamp} milliseconds.");
                }
                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = GetNextTimestamp();
                    }
                }
                else
                {
                    _sequence = 0;
                }
                _lastTimestamp = timestamp;
                return (timestamp - Twepoch << TimestampLeftShift) | (_datacenterId << DatacenterIdShift) | (_workerId << WorkerIdShift) | _sequence;
            }
        }

        /// <summary>
        /// 获取下一个时间戳
        /// </summary>
        /// <returns></returns>
        protected virtual long GetNextTimestamp()
        {
            var timestamp = GetCurTimestamp();
            while (timestamp <= _lastTimestamp)
            {
                timestamp = GetCurTimestamp();
            }
            return timestamp;
        }

        //private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 生成当前时间戳（毫秒）
        /// </summary>
        /// <returns></returns>
        protected virtual long GetCurTimestamp()
        {
            //return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000;
        }
    }
}
