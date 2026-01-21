using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nominatim.API.Models
{

    public sealed class OverpassElement
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = ""; // node|way|relation

        [JsonPropertyName("id")]
        public long Id { get; set; }

        // nodes:
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        // ways/relations:
        [JsonPropertyName("center")]
        public OverpassCenter Center { get; set; }

        [JsonPropertyName("tags")]
        public Dictionary<string, string> Tags { get; set; }
    }
}
