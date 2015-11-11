using System;

namespace Td.AspNet.Utils
{
    public class IDCreater
    {
        /**
         本程序可以生成64位全局唯一ID，适合在分布式服务器集群中产生唯一ID
       　特点比微软自带的GUID要节省一倍的空间，即只需要64位的int即可，因此基本上可以抛弃微软的那个GUID了
         支持每秒生成4096个,在此条件基础上这辈子都不会产生相同的ID

         由 时间戳+服务器号+平台号+本地递增序号 组成

         时间戳 32bit
         服务器号 12bit
         平台号 8bit
         递增序号 12bit 
       */
        private static ulong mark_timestamp = 0xffffffff00000000;/*时间戳掩码*/
        private static ulong mark_district = 0x00000000fff00000;/*服务器号掩码*/
        private static ulong mark_plat = 0x00000000000ff000;/*服务器里的平台号掩码*/
        private static ulong mark_base = 0x0000000000000fff;/*本地ID编号掩码*/
        private static uint baseId = 0;/*本地ID*/

        //生成整个世界里全局唯一的ID district：服务器号，platform：平台号 [如果只有一个平台，则此平台号可以是服务器集群里的服务器类型编号]
        public static ulong NewId(uint platform, uint district)
        {
            ulong timeStamp = (ulong)TimeGen();
            ulong newId = ((timeStamp << 32) & mark_timestamp) | ((district << 20) & mark_district) | ((platform << 12) & mark_plat) | (baseId & mark_base);
            baseId++;
            return newId;
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// //获取自古以来的时间戳
        /// </summary>
        /// <returns></returns>
        private static long TimeGen()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalSeconds;
        }
    }
}
