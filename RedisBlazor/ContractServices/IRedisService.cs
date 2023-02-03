using Microsoft.Extensions.Caching.Distributed;

namespace RedisBlazor.ContractServices
{
    public interface IRedisService
    {
        Task SetRecordAsync<T>(string recordId,T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null);
        Task<T?> GetRecordAsync<T>(string recordId);

        ValueTask<bool> PublishAsyc<T>(T data, string channel);
        Task<T> SubscribeAsync<T>(string channel);
    }
}
