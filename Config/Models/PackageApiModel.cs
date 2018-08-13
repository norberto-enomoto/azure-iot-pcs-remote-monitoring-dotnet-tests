// Copyright (c) Microsoft. All rights reserved.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Config
{
    public class PackageApiModel
    {
        private const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PackageType Type { get; set; }

        [JsonProperty(PropertyName = "DateCreated")]
        public string DateCreated { get; set; } = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);

    }

    public enum PackageType
    {
        EDGE_MANIFEST
    }
}
