using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nominatim.API.Models
{
    public sealed record PoiTagFilter(string Key, string Value);
}
