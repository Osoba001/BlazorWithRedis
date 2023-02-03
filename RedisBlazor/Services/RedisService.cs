using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisBlazor.ContractServices;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisBlazor.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;

        public RedisService(IConfiguration config,IDistributedCache cache)
        {
            _config = config;
            _cache = cache;
        }
        public async Task<T?> GetRecordAsync<T>(string recordId)
        {
            var data = await _cache.GetStringAsync(recordId);
            return data != null ? System.Text.Json.JsonSerializer.Deserialize<T>(data) : default;
        }

       

        public async Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var option = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                SlidingExpiration = unusedExpireTime
            };
            var jsData = System.Text.Json.JsonSerializer.Serialize(data);
            await _cache.SetStringAsync(recordId, jsData, option);
        }


        public async ValueTask<bool> PublishAsyc<T>(T data, string channel)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var connString = _config.GetConnectionString("Redis");
            var connection = ConnectionMultiplexer.Connect(connString);
            return await connection.GetSubscriber().PublishAsync(channel, jsonData) > 0;
        }
        public async Task<T?> SubscribeAsync<T>(string channel)
        {
            T messageType=default;
            var connString = _config.GetConnectionString("Redis");
            var connection = ConnectionMultiplexer.Connect(connString);
             await connection.GetSubscriber().SubscribeAsync(channel, (channel, message) =>
            {
                if (!message.IsNull)
                {
                    var res = JsonConvert.DeserializeObject<T>(value: message);
                    if (res != null)
                        messageType = res;
                }
            });
            return messageType;
        }
    }
}
