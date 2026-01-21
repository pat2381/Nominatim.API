using Nominatim.API.Models;
using System.Collections.Generic;
using XENBIT.ResQueServe.Abstractions.DataStructures;

namespace Nominatim.API.Interfaces
{
    public interface IPoiCatalog
    {
        IReadOnlyList<TagSet> GetQueryTagSets(PoiCategory category);
    }
}
