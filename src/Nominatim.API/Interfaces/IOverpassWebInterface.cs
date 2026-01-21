using Nominatim.API.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Interfaces
{
    public interface IOverpassWebInterface
    {
        Task<OverpassResponse> QueryAsync(string overpassQl, CancellationToken ct = default);
    }
}
