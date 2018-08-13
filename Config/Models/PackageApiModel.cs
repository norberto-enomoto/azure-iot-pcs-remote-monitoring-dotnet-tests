// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Config
{
    public class PackageApiModel
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PackageType Type { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }

    }

    public enum PackageType
    {
        EDGE_MANIFEST
    }
}
