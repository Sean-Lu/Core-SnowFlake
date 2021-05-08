## 简介：雪花算法(SnowFlake)

> `SnowFlake`是`Twitter`开源的分布式ID生成算法。其核心思想就是：使用一个64位的long类型的数字作为全局唯一id。

1. 自增ID：对于数据敏感场景不宜使用，且不适合于分布式场景。
2. GUID：采用无意义字符串，数据量增大时造成访问过慢，且不宜排序。
3. SnowFlake：

```
最高位是符号位，始终为0，不可用。
41位的时间戳（精确到毫秒，41位的长度可使用69年）
10位的机器ID（10位长度最多支持1024个服务器节点部署）
12位的计数序列号（序列号即一系列的自增id，可以支持同一节点同一毫秒生成多个ID序号，12位支持每节点每毫秒最多生成4096个序列号）
```

- 优点：

```
1. 生成的ID整体上按照时间自增排序，并且整个分布式系统内不会产生ID碰撞（由datacenterId和workerId作区分）。
2. 基于二进制运算，生成效率较高（据说：snowflake每秒能够产生26万个ID）。
3. 算法实现简单，还可根据实际业务场景对算法做微调（位数）。
```

- 缺点：

```
1. 强依赖时钟，如果主机时间回拨，则可能会造成重复ID（需要主动抛出异常）。
2. ID虽然有序，但是不连续（只要没有强迫症，似乎不是什么问题，毕竟保证ID全局唯一才是重点）。

snowflake现在有较好的改良方案，比如美团点评开源的分布式ID框架：leaf，通过使用ZooKeeper解决了时钟依赖问题。
```

- 使用场景：主要用于分布式系统中，生成全局唯一的id，如：订单号等。

## Packages

| Package | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- |
| [Sean.Core.SnowFlake](https://www.nuget.org/packages/Sean.Core.SnowFlake/) | [![Sean.Core.SnowFlake](https://img.shields.io/nuget/v/Sean.Core.SnowFlake.svg)](https://www.nuget.org/packages/Sean.Core.SnowFlake/) | [![Sean.Core.SnowFlake](https://img.shields.io/nuget/vpre/Sean.Core.SnowFlake.svg)](https://www.nuget.org/packages/Sean.Core.SnowFlake/) | [![Sean.Core.SnowFlake](https://img.shields.io/nuget/dt/Sean.Core.SnowFlake.svg)](https://www.nuget.org/packages/Sean.Core.SnowFlake/) |

## Nuget包引用

> **Id：Sean.Core.SnowFlake**

- Package Manager

```
PM> Install-Package Sean.Core.SnowFlake
```

## 使用示例

> 项目：test\Demo.NetCore
