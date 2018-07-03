// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using Helpers.Http;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Telemetry
{
    public class RulesTest
    {
        private readonly IHttpClient httpClient;
        private const string TELEMETRY_ADDRESS = "http://127.0.0.1:9004/v1";
        private string instantRuleId;
        private string average1MinRuleId;
        private string average5MinRuleId;
        private string average10MinRuleId;

        public RulesTest()
        {
            this.httpClient = new HttpClient();

            // setup unique ids for rules
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            const string TEST_RULE_PREFIX = "Integration_Test_";
            this.instantRuleId = TEST_RULE_PREFIX + "Instant_" + timestamp;
            this.average1MinRuleId = TEST_RULE_PREFIX + "Average_1_Min_" + timestamp;
            this.average5MinRuleId = TEST_RULE_PREFIX + "Average_5_Min_" + timestamp;
            this.average10MinRuleId = TEST_RULE_PREFIX + "Average_10_Min_" + timestamp;
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that the service starts normally and returns ok status
        /// </summary>
        [Fact, Trait("Type", "IntegrationTest")]
        public void Should_Return_OK_Status()
        {
            // Act
            var request = new HttpRequest(TELEMETRY_ADDRESS + "/status");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact, Trait("Type", "IntegrationTest")]
        public void CreatesRuleWithInstantCalculation_IfValid()
        {
            // Arrange
            // wait for config to run seed data for device groups
            //System.Threading.Thread.Sleep(60000);

            // Create a rule with instant calculation
            var body = JObject.Parse(@"{  
               'Name': 'Instant Rule',
               'Description': 'Instant Description',
               'GroupId': 'default_Chillers',
               'Severity': 'info',
               'Enabled': true,
               'Calculation': 'Instant',
               'TimePeriod': '0',
               'Conditions': [  
                  {  
                     'Field': 'pressure',
                     'Operator': 'GreaterThan',
                     'Value': '150'
                  }
               ]
            }");

            // Act
            var request = new HttpRequest(TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(body);

            var response = this.httpClient.PostAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            JObject jsonResponse = JObject.Parse(response.Content);
            Assert.True(jsonResponse.HasValues);

            /*
            JArray items = (JArray)jsonResponse["Items"];
            //TODO: Make it equal to 5 once fresh storage is created
            //Since we are using same storage account for now this number should be great or equal to 5
            Assert.True(items.Count >= 4);

            List<string> groupIds = new List<string>();
            foreach (var rule in items)
            {
                groupIds.Add(rule["GroupId"].ToString());
            }

            Assert.Contains("default_Chillers", groupIds);
            Assert.Contains("default_PrototypingDevices", groupIds);
            Assert.Contains("default_Trucks", groupIds);
            Assert.Contains("default_Elevators", groupIds);
            */
        }
    }
}
