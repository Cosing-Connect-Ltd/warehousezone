using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Ganedata.Core.Entities.Domain.Models
{
    public class MerakiScanningApiPayload
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
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

        [JsonProperty("latestRecord")]
        public LatestRecord LatestRecord { get; set; }

        [JsonProperty("ipv6")]
        public object Ipv6 { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }
    }

    public partial class LatestRecord
    {
        [JsonProperty("time")]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("nearestApMac")]
        public string NearestApMac { get; set; }

        [JsonProperty("nearestApRssi")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long NearestApRssi { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("x")]
        public string X { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }

        [JsonProperty("floorPlanName")]
        public string FloorPlanName { get; set; }

        [JsonProperty("rssiRecords")]
        public RssiRecord[] RssiRecords { get; set; }

        [JsonProperty("variance")]
        public double Variance { get; set; }

        [JsonProperty("y")]
        public string Y { get; set; }

        [JsonProperty("nearestApTags")]
        public NearestApTag[] NearestApTags { get; set; }

        [JsonProperty("floorPlanId")]
        public string FloorPlanId { get; set; }

        [JsonProperty("time")]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }
    }

    public partial class RssiRecord
    {
        [JsonProperty("apMac")]
        public string ApMac { get; set; }

        [JsonProperty("rssi")]
        public long Rssi { get; set; }
    }

    public enum NearestApTag { ApiTest, Empty, Office };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                NearestApTagConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class NearestApTagConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(NearestApTag) || t == typeof(NearestApTag?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return NearestApTag.Empty;
                case "API-TEST":
                    return NearestApTag.ApiTest;
                case "Office":
                    return NearestApTag.Office;
            }
            throw new Exception("Cannot unmarshal type NearestApTag");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (NearestApTag)untypedValue;
            switch (value)
            {
                case NearestApTag.Empty:
                    serializer.Serialize(writer, "");
                    return;
                case NearestApTag.ApiTest:
                    serializer.Serialize(writer, "API-TEST");
                    return;
                case NearestApTag.Office:
                    serializer.Serialize(writer, "Office");
                    return;
            }
            throw new Exception("Cannot marshal type NearestApTag");
        }

        public static readonly NearestApTagConverter Singleton = new NearestApTagConverter();
    }
}