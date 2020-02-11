using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Ganedata.Core.Entities.Domain.Models
{
    public class MerakiScanningApiViewModel
    {
        public string Version { get; set; }
        public string Secret { get; set; }
        public string Type { get; set; }
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("apMac")]
        public string ApMac { get; set; }

        [JsonProperty("apTags")]
        public string[] ApTags { get; set; }

        [JsonProperty("networkId")]
        public string NetworkId { get; set; }

        [JsonProperty("observations")]
        public Observation[] Observations { get; set; }
    }

    public partial class Observation
    {
        [JsonProperty("locations")]
        public Location[] Locations { get; set; }

        [JsonProperty("ipv4")]
        public string Ipv4 { get; set; }

        [JsonProperty("ssid")]
        public string Ssid { get; set; }

        [JsonProperty("os")]
        public string Os { get; set; }

        [JsonProperty("clientMac")]
        public string ClientMac { get; set; }

        [JsonProperty("ipv6")]
        public object Ipv6 { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("rssi")]
        public short rssi { get; set; }

        [JsonProperty("seenEpoch")]
        public long SeenEpoch { get; set; }

        [JsonProperty("seenTime")]
        public DateTime? SeenTime { get; set; }

    }

    public partial class Location
    {
        [JsonProperty("x")]
        public string X { get; set; }
        [JsonProperty("y")]
        public string Y { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }

        [JsonProperty("unc")]
        public DateTimeOffset Unc { get; set; }

    }
}