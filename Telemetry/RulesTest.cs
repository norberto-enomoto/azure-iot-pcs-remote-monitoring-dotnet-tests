// Copyright (c) Microsoft. All rights reserved.

using System;
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
        private const string CONFIG_ADDRESS = "http://127.0.0.1:9005/v1";
        private const string DEFAULT_CHILLERS_GROUP_ID = "default_Chillers";
        private const int SEED_DATA_RETRY_COUNT = 5;
        private const int SEED_DATA_RETRY_MSEC = 10000;

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

            // Make sure device groups have been created by seed data
            Assert.True(this.ValidChillerGroup());

            // Create a rule with instant calculation
            var body = JObject.Parse(@"{  
               'Name': 'Instant Rule',
               'Description': 'Instant Description',
               'GroupId': '" + DEFAULT_CHILLERS_GROUP_ID + @"',
               'Severity': 'Info',
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
            var jsonResponse = JObject.Parse(response.Content);
            Assert.True(jsonResponse.HasValues);
            Assert.Equal(body.GetValue("Name"), jsonResponse.GetValue("Name"));
            Assert.Equal(body.GetValue("Description"), jsonResponse.GetValue("Description"));
            Assert.Equal(body.GetValue("GroupId"), jsonResponse.GetValue("GroupId"));
            Assert.Equal(body.GetValue("Severity"), jsonResponse.GetValue("Severity"));
            Assert.Equal(body.GetValue("Enabled"), jsonResponse.GetValue("Enabled"));
            Assert.Equal(body.GetValue("Calculation"), jsonResponse.GetValue("Calculation"));
            Assert.Equal(body.GetValue("Conditions"), jsonResponse.GetValue("Conditions"));
        }

        /// <summary>
        /// Returns true if the default chiller device group has been created by seed data.
        /// Retries with a 10 sec timer if seed data in config service has not yet
        /// created the device groups. Returns false after SEED_DATA_RETRY_COUNT failed attempts.
        /// </summary>
        private bool ValidChillerGroup()
        {
            for (var i = 0; i < SEED_DATA_RETRY_COUNT; i++)
            {
                var chillerRequest = new HttpRequest(CONFIG_ADDRESS + "/devicegroups/" + DEFAULT_CHILLERS_GROUP_ID);
                chillerRequest.AddHeader("X-Foo", "Bar");

                var response = this.httpClient.GetAsync(chillerRequest).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                // wait 10 seconds before retry if able
                if (i < SEED_DATA_RETRY_COUNT-1) System.Threading.Thread.Sleep(SEED_DATA_RETRY_MSEC);
            }

            return false;
        } 
    }
}
