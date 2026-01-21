using Nominatim.API.Interfaces;
using Nominatim.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Contracts
{
    public sealed class NominatimAreaResolver : IAreaResolver
    {
        private readonly IForwardGeocoder _forwardGeocoder;
        private readonly IKeyValueCache _cache; // siehe Cache unten
        private readonly IRateLimiter _rateLimiter;

        public NominatimAreaResolver(
            IForwardGeocoder forwardGeocoder,
            IKeyValueCache cache,
            IRateLimiter rateLimiter)
        {
            _forwardGeocoder = forwardGeocoder;
            _cache = cache;
            _rateLimiter = rateLimiter;
        }

        public async Task<AreaResolution> ResolveAsync(string areaName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(areaName))
                throw new ArgumentException("Area name is empty.");

            var cacheKey = $"area:resolve:{areaName.Trim().ToLowerInvariant()}";
            var cached = await _cache.GetAsync<AreaResolution>(cacheKey, ct);
            if (cached != null) return cached;

            await _rateLimiter.WaitAsync(ct);

            var results = await _forwardGeocoder.Geocode(new ForwardGeocodeRequest
            {
                queryString = areaName,
                ShowExtraTags = true,
                LimitResults = 10
            });

            if (results == null || results.Length == 0)
                return null;

            AreaResolution best = null;

            foreach (var r in results)
            {
                var candidate = ToCandidate(areaName, r);
                if (candidate == null) continue;

                if (best == null || candidate.Score > best.Score)
                    best = candidate;
            }

            if (best == null)
                return null;

            await _cache.SetAsync(cacheKey, best, TimeSpan.FromDays(14), ct);
            return best;
        }

        private static AreaResolution ToCandidate(string query, GeocodeResponse r)
        {
            // TODO: Diese Property Namen musst du anpassen:
            // r.DisplayName, r.Class, r.Type, r.BoundingBox...
            var displayName = r.DisplayName ?? "";
            if (string.IsNullOrWhiteSpace(displayName))
                return null;

            var bbox = TryGetBoundingBox(r);
            if (bbox == null)
                return null;

            var score = 0;
            var reason = new List<string>();

            var cls = (r.Class ?? "").Trim().ToLowerInvariant();
            var typ = (r.Type ?? "").Trim().ToLowerInvariant();

            // boundary administrative ist meist das, was du willst
            if (cls == "boundary") { score += 40; reason.Add("class=boundary"); }
            if (typ == "administrative") { score += 40; reason.Add("type=administrative"); }

            // Query Treffer
            if (displayName.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                score += 15;
                reason.Add("display_name contains query");
            }

            // Bonus wenn es nach Kreis klingt
            if (displayName.Contains("Kreis", StringComparison.OrdinalIgnoreCase) ||
                displayName.Contains("Landkreis", StringComparison.OrdinalIgnoreCase))
            {
                score += 10;
                reason.Add("display_name hints county");
            }

            // Optional: wenn du OsmType/OsmId ergänzt, wird das hier aktiv
            long? relationId = null;

            // Wenn du GeocodeResponse erweiterst:
            // if (string.Equals(r.OsmType, "relation", StringComparison.OrdinalIgnoreCase))
            // {
            //     score += 30;
            //     relationId = r.OsmId;
            //     reason.Add("osm_type=relation");
            // }

            return new AreaResolution(
                Query: query,
                DisplayName: displayName,
                RelationId: relationId,
                BoundingBox: (BoundingBox)bbox,
                Score: score,
                Reason: string.Join(", ", reason)
            );
        }

        private static BoundingBox? TryGetBoundingBox(GeocodeResponse r)
        {
            // TODO: Das hängt am Repo Modell.
            // Manche Modelle haben r.BoundingBox als string array. Manche als doubles.

            // Beispiel, wenn du vier doubles hast:
            // return new BoundingBox(r.BoundingBoxSouth, r.BoundingBoxWest, r.BoundingBoxNorth, r.BoundingBoxEast);

            // Fallback: wenn es ein string[] ist: [south, north, west, east] oder ähnlich
            // Bitte im Debug einmal anschauen und dann korrekt mappen.

            return null;
        }
    }
}
