namespace Sean.Core.SnowFlake.Contracts
{
    public interface IIdManager
    {
        /// <summary>
        /// 数据中心ID(0~31)
        /// </summary>
        long DatacenterId { get; set; }
        /// <summary>
        /// 工作机器ID(0~31)
        /// </summary>
        long WorkerId { get; set; }

        /// <summary>
        /// 获得下一个ID (线程安全) 
        /// </summary>
        /// <returns>全局唯一的id</returns>
        long NextId();
    }
    public interface IIdManager<T> : IIdManager where T : class
    {

    }
}
