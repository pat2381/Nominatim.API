using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Interfaces
{
    public interface IRateLimiter
    {
        Task WaitAsync(CancellationToken ct = default);
    }
}
