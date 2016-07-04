using Microsoft.Extensions.Caching.Memory;


namespace Td.AspNet.Utils
{
    /// <summary>
    /// 本地缓存
    /// </summary>
    public class LocalCacheProvider
    {
        private static IMemoryCache _cache;

        static IMemoryCache GetInstance
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new MemoryCache(new MemoryCacheOptions());
                }
                return _cache;
            }
        }

        public static object Get(string key)
        {
            return GetInstance.Get(key);
        }

        public static T Get<T>(string key)
        {
            return GetInstance.Get<T>(key);
        }


        public static void Set(string key, object value)
        {
            GetInstance.Set(key, value);
        }
    }
}
