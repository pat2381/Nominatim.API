using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nominatim.API.Models
{
    // Ergebnis auf “für Apps nutzbar” reduziert:
    public sealed record PoiResult(
      long OsmId,
      string OsmType,
      double Lat,
      double Lon,
      IReadOnlyDictionary<string, string> Tags
  );
}
