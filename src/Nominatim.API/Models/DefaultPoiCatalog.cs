using Nominatim.API.Filters;
using Nominatim.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XENBIT.ResQueServe.Abstractions.DataStructures;

namespace Nominatim.API.Models
{
    public sealed class DefaultPoiCatalog : IPoiCatalog
    {
        private static readonly Dictionary<PoiCategory, IReadOnlyList<TagSet>> Map = new()
        {
            [PoiCategory.Kindergarten] = new List<TagSet>
        {
            new TagSet(new TagFilter("amenity", "kindergarten")),
            new TagSet(new TagFilter("amenity", "childcare")),
            new TagSet(new TagFilter("building", "kindergarten"))
        },

            [PoiCategory.FireStation] = new List<TagSet>
        {
            new TagSet(new TagFilter("amenity", "fire_station"))
        },

            [PoiCategory.Hospital] = new List<TagSet>
        {
            new TagSet(new TagFilter("amenity", "hospital"))
        },

            [PoiCategory.PoliceStation] = new List<TagSet>
        {
            new TagSet(new TagFilter("amenity", "police"))
        },

            [PoiCategory.ShoppingCenter] = new List<TagSet>
        {
            new TagSet(new TagFilter("shop", "supermarket"))
        }
        };

        public IReadOnlyList<TagSet> GetQueryTagSets(PoiCategory category)
            => Map.TryGetValue(category, out var sets) ? sets : Array.Empty<TagSet>();
    }
}
