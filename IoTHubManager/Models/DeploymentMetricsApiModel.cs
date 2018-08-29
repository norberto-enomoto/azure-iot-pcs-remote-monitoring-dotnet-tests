// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IoTHubManager
{
    public class DeploymentMetricsApiModel
    {
        [JsonProperty(PropertyName = "AppliedCount")]
        public long AppliedCount { get; set; }
        
        [JsonProperty(PropertyName = "FailedCount")]
        public long FailedCount { get; set; }

        [JsonProperty(PropertyName = "SucceededCount")]
        public long SucceededCount { get; set; }

        [JsonProperty(PropertyName = "TargetedCount")]
        public long TargetedCount { get; set; }

        public DeploymentMetricsApiModel()
        {
        }
    }
}
