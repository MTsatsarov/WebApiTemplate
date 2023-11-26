namespace WebApiTemplate.Services.Infrastructure.Cache
{
	public interface ICacheService
	{
		/// <summary>
		/// Gets value from a cache. If the value is not presented in the cache sets the result from the fallbackFunc and returns it.
		/// </summary>
		/// <param name="key">The key which holds the value in the cache.</param>
		/// <param name="fallbackFunc">The function that will be called in case of exception or value not presenting.</param>
		/// <param name="cacheExpirationInSeconds">The time in seconds which will determine how long the value must be kept.</param>
		Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fallbackFunc, int? cacheExpirationInSeconds = null);

		/// <summary>
		/// Removes the key from the cache.
		/// In case of key not presenting, nothing happens.
		/// </summary>
		/// <param name="key">The key which needs to be removed..</param>
		bool Remove(string key);
	}
}
