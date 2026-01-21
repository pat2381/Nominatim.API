using Nominatim.API.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.RateLimiter
{
    public sealed class FixedDelayRateLimiter : IRateLimiter
    {
        private readonly SemaphoreSlim _mutex = new(1, 1);
        private readonly TimeSpan _minDelay;
        private DateTime _lastUtc = DateTime.MinValue;

        public FixedDelayRateLimiter(TimeSpan minDelay)
        {
            if (minDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(minDelay));
            _minDelay = minDelay;
        }

        public async Task WaitAsync(CancellationToken ct = default)
        {
            await _mutex.WaitAsync(ct);
            try
            {
                var now = DateTime.UtcNow;
                var next = _lastUtc + _minDelay;
                if (next > now)
                {
                    var delay = next - now;
                    await Task.Delay(delay, ct);
                }

                _lastUtc = DateTime.UtcNow;
            }
            finally
            {
                _mutex.Release();
            }
        }
    }
}
