// Copyright (c) Microsoft. All rights reserved.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IoTHubManager
{
    public enum DeploymentType {
        EdgeManifest
    }

    public class DeploymentApiModel
    {
        [JsonProperty(PropertyName = "CreatedDateTimeUtc")]
        public DateTime CreatedDateTimeUtc { get; set; }

        [JsonProperty(PropertyName = "DeviceGroupId")]
        public string DeviceGroupId { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

         [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "PackageId")]
        public string PackageId { get; set; }
        
        [JsonProperty(PropertyName = "Priority")]
        public int Priority { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "Type")]
        public DeploymentType Type { get; set; }

        [JsonProperty(PropertyName = "Metrics", NullValueHandling = NullValueHandling.Ignore)]
        public DeploymentMetricsApiModel Metrics { get; set; }
    }
}
