using FluentExtensions.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using WebApiTemplate.Common.Helpers;

namespace WebApiTemplate.Services.Infrastructure.Cache
{
	public class RedisCacheService : ICacheService
	{
		private readonly IDistributedCache cache;
		private readonly IConnectionMultiplexer redisConnection;

		public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redisConnection)
		{
			this.cache = cache;
			this.redisConnection = redisConnection;
		}

		public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fallbackFunc, int? cacheExpirationInSeconds = null)
		{
			if (!RedisIsConnected())
			{
				return await fallbackFunc();
			}

			try
			{
				T value = await this.GetOrSetValue<T>(key, fallbackFunc, cacheExpirationInSeconds.Value);
				return value;
			}
			catch
			{
				return await fallbackFunc();
			}

		}

		public async Task Remove(string key)
		{
			if (RedisIsConnected())
			{
				await this.cache.RemoveAsync(key);
			}
		}

		private bool RedisIsConnected() => this.redisConnection.IsConnected;

		private async Task<T> GetOrSetValue<T>(string key, Func<Task<T>> fallbackFunc, int? cacheExpiration)
		{
			byte[] value = await this.cache.GetAsync(key);
			if (value is null)
			{
				DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();

				if (cacheExpiration.HasValue)
				{
					options.SetAbsoluteExpiration(DateTime.Now.AddSeconds(cacheExpiration.Value));
				}

				T result = await fallbackFunc();
				await this.cache.SetAsync(key, ParseToByteArray(result), options);

				return result;
			}

			return ParseToGeneric<T>(value);
		}

		private static byte[] ParseToByteArray<T>(T obj)
					=> obj!.ToJson().ToByteArray();

		private static T ParseToGeneric<T>(byte[] byteArray)
			=> new StreamReader(new MemoryStream(byteArray)).ReadToEnd().FromJson<T>();
	}
}
