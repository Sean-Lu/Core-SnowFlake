namespace Sean.Core.SnowFlake.Contracts
{
    public interface IIdManager
    {
        /// <summary>
        /// ��������ID(0~31)
        /// </summary>
        long DatacenterId { get; set; }
        /// <summary>
        /// ��������ID(0~31)
        /// </summary>
        long WorkerId { get; set; }

        /// <summary>
        /// �����һ��ID (�̰߳�ȫ) 
        /// </summary>
        /// <returns>ȫ��Ψһ��id</returns>
        long NextId();
    }
    public interface IIdManager<T> : IIdManager where T : class
    {

    }
}
