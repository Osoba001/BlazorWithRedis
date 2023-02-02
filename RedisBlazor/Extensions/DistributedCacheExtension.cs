using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisBlazor.Extensions
{
    public static class DistributedCacheExtension
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime=null,
            TimeSpan? unusedExpireTime = null)
        {
            var option = new DistributedCacheEntryOptions();
            option.AbsoluteExpirationRelativeToNow = absoluteExpireTime?? TimeSpan.FromSeconds(60);
            option.SlidingExpiration = unusedExpireTime;
            var jsData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId,jsData, option);
        }

        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {

            try
            {
                var jsonData = await cache.GetStringAsync(recordId);
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
           

           

            
            //var data=await cache.GetStringAsync(recordId);
            //return data != null ? JsonSerializer.Deserialize<T>(data) : default;
        }
    }
}
