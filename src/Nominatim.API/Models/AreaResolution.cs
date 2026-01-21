using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nominatim.API.Models
{
    public sealed record AreaResolution(
        string Query,
        string DisplayName,
        long? RelationId,
        BoundingBox BoundingBox,
        int Score,
        string Reason
    );
}
