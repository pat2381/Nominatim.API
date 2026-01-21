using Nominatim.API.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nominatim.API.Builder
{
    public static class OverpassQueryBuilder
    {
        public static string BuildInArea(long relationId, IReadOnlyList<TagSet> tagSets, int timeoutSeconds)
        {
            var areaId = 3600000000L + relationId;

            return $@"
                    [out:json][timeout:{timeoutSeconds}];
                    area({areaId})->.searchArea;
                    (
                    {BuildBody(tagSets, "area.searchArea")}
                    );
                    out center;";
        }

        public static string BuildInBoundingBox(BoundingBox bbox, IReadOnlyList<TagSet> tagSets, int timeoutSeconds)
        {
            var bb = $"{bbox.minLatitude.ToString(CultureInfo.InvariantCulture)}," +
                     $"{bbox.minLongitude.ToString(CultureInfo.InvariantCulture)}," +
                     $"{bbox.maxLatitude.ToString(CultureInfo.InvariantCulture)}," +
                     $"{bbox.maxLongitude.ToString(CultureInfo.InvariantCulture)}";

            return $@"
                        [out:json][timeout:{timeoutSeconds}];
                        (
                        {BuildBody(tagSets, bb)}
                        );
                        out center;";
        }

        private static string BuildBody(IReadOnlyList<TagSet> tagSets, string areaOrBbox)
        {
            if (tagSets == null || tagSets.Count == 0)
                throw new ArgumentException("No tag sets provided.");

            // OR über TagSets
            // AND innerhalb eines TagSets
            var lines = new List<string>();

            foreach (var set in tagSets)
            {
                var filter = BuildAndFilters(set);
                lines.Add($"  node{filter}({areaOrBbox});");
                lines.Add($"  way{filter}({areaOrBbox});");
                lines.Add($"  relation{filter}({areaOrBbox});");
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string BuildAndFilters(TagSet set)
        {
            // AND: node["k"="v"]["k2"="v2"]
            var sb = new System.Text.StringBuilder();
            foreach (var f in set.Filters)
            {
                sb.Append(@"[""");
                sb.Append(Escape(f.Key));
                sb.Append(@"""=""");
                sb.Append(Escape(f.Value));
                sb.Append(@"""]");
            }
            return sb.ToString();
        }

        private static string Escape(string s) => (s ?? "").Replace(@"\", @"\\").Replace(@"""", @"\""");
    }
}

