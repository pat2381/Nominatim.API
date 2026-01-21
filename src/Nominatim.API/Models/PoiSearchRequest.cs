
using XENBIT.ResQueServe.Abstractions.DataStructures;

namespace Nominatim.API.Models
{
    public sealed record PoiSearchRequest(
    string AreaName,
    PoiCategory Category,
    int TimeoutSeconds = 60
);
}
