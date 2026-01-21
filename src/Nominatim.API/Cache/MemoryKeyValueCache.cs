using Microsoft.Extensions.Caching.Memory;
using Nominatim.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Cache
{
    public sealed class MemoryKeyValueCache : IKeyValueCache
    {
        private readonly IMemoryCache _cache;

        public MemoryKeyValueCache(IMemoryCache cache) => _cache = cache;

        public Task<T> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        {
            _cache.TryGetValue(key, out T value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class
        {
            _cache.Set(key, value, ttl);
            return Task.CompletedTask;
        }
    }
}
