using Nominatim.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Interfaces
{
    public interface IAreaResolver
    {
        Task<AreaResolution> ResolveAsync(string areaName, CancellationToken ct = default);
    }
}
