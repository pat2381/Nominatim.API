using Nominatim.API.Builder;
using Nominatim.API.Interfaces;
using Nominatim.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Address
{
    public sealed class PoiSearcher : IPoiSearcher
    {
        private readonly IAreaResolver _areaResolver;
        private readonly IPoiCatalog _catalog;
        private readonly IOverpassWebInterface _overpass;
        private readonly IKeyValueCache _cache;
        private readonly IRateLimiter _overpassLimiter;

        public PoiSearcher(
            IAreaResolver areaResolver,
            IPoiCatalog catalog,
            IOverpassWebInterface overpass,
            IKeyValueCache cache,
            IRateLimiter overpassLimiter)
        {
            _areaResolver = areaResolver;
            _catalog = catalog;
            _overpass = overpass;
            _cache = cache;
            _overpassLimiter = overpassLimiter;
        }

        public async Task<IReadOnlyList<PoiResult>> SearchAsync(PoiSearchRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.AreaName))
                throw new ArgumentException("AreaName is empty.");

            var tagSets = _catalog.GetQueryTagSets(request.Category);
            if (tagSets.Count == 0)
                return Array.Empty<PoiResult>();

            var area = await _areaResolver.ResolveAsync(request.AreaName, ct);
            if (area == null)
                return Array.Empty<PoiResult>();

            var cacheKey = $"poi:{request.Category}:{area.DisplayName}:{area.RelationId}:{area.BoundingBox}";
            var cached = await _cache.GetAsync<List<PoiResult>>(cacheKey, ct);
            if (cached != null) return cached;

            var ql = area.RelationId.HasValue
                ? OverpassQueryBuilder.BuildInArea(area.RelationId.Value, tagSets, request.TimeoutSeconds)
                : OverpassQueryBuilder.BuildInBoundingBox(area.BoundingBox, tagSets, request.TimeoutSeconds);

            await _overpassLimiter.WaitAsync(ct);
            var resp = await _overpass.QueryAsync(ql, ct);

            var mapped = Map(resp);
            await _cache.SetAsync(cacheKey, mapped, TimeSpan.FromMinutes(30), ct);

            return mapped;
        }

        private static List<PoiResult> Map(OverpassResponse resp)
        {
            var list = new List<PoiResult>(resp.Elements.Count);

            foreach (var e in resp.Elements)
            {
                var lat = e.Lat != 0 ? e.Lat : e.Center?.Lat;
                var lon = e.Lon != 0 ? e.Lon : e.Center?.Lon;
                if (lat is null || lon is null) continue;

                list.Add(new PoiResult(
                    OsmId: e.Id,
                    OsmType: e.Type,
                    Lat: lat.Value,
                    Lon: lon.Value,
                    Tags: e.Tags ?? new Dictionary<string, string>()
                ));
            }

            return list;
        }
    }
}
