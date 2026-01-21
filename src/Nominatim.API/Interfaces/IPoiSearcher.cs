using Nominatim.API.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Interfaces
{
    public interface IPoiSearcher
    {
        Task<IReadOnlyList<PoiResult>> SearchAsync(PoiSearchRequest request, CancellationToken ct = default);
    }
}

