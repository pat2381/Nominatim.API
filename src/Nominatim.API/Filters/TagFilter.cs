using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nominatim.API.Filters
{
    public sealed record TagFilter(string Key, string Value);
}
