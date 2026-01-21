using Nominatim.API.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nominatim.API.Models
{
    public sealed class TagSet
    {
        public IReadOnlyList<TagFilter> Filters { get; }

        public TagSet(params TagFilter[] filters)
        {
            if (filters is null || filters.Length == 0)
                throw new ArgumentException("TagSet needs at least one filter.");

            Filters = filters;
        }
    }
}
