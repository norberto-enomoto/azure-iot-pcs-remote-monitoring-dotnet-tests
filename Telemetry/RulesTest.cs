// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using Helpers;
using Helpers.Models;
using Helpers.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Telemetry
{
    public class RulesTest
    {

        private readonly IHttpClient httpClient;

        private const string DEFAULT_CHILLERS_GROUP_ID = "default_Chillers";
        private const int SEED_DATA_RETRY_COUNT = 5;
        private const int SEED_DATA_RETRY_MSEC = 10000;

        // list of rules to delete when tests are complete
        private List<string> rulesCreated;

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

            this.rulesCreated = new List<string>();
        }

        ~RulesTest()
        {
            foreach (var ruleId in this.rulesCreated)
            {
                var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + ruleId);
                request.AddHeader("X-Foo", "Bar");

                var response = this.httpClient.DeleteAsync(request).Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("Unable to delete test rule id:" + ruleId);
                }
            }
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that the service starts normally and returns ok status
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Should_Return_OK_Status()
        {
            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/status");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CreatesRuleWithInstantCalculation_IfValid()
        {
            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Instant", "0");

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));


            var response = this.httpClient.PostAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ruleRequest.Name, ruleResponse.Name);
            Assert.Equal(ruleRequest.Description, ruleResponse.Description);
            Assert.Equal(ruleRequest.GroupId, ruleResponse.GroupId);
            Assert.Equal(ruleRequest.Severity, ruleResponse.Severity);
            Assert.Equal(ruleRequest.Enabled, ruleResponse.Enabled);
            Assert.Equal(ruleRequest.Calculation, ruleResponse.Calculation);
            Assert.Equal(ruleRequest.Conditions[0].Field, ruleResponse.Conditions[0].Field);
            Assert.Equal(ruleRequest.Conditions[0].Operator, ruleResponse.Conditions[0].Operator);
            Assert.Equal(ruleRequest.Conditions[0].Value, ruleResponse.Conditions[0].Value);

            this.rulesCreated.Add(ruleResponse.Id); // Track new rule for deletion
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CreatesRuleWithAvg1MinCalculation_IfValid()
        {
            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Average", "60000");

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));

            var response = this.httpClient.PostAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ruleRequest.Name, ruleResponse.Name);
            Assert.Equal(ruleRequest.Description, ruleResponse.Description);
            Assert.Equal(ruleRequest.GroupId, ruleResponse.GroupId);
            Assert.Equal(ruleRequest.Severity, ruleResponse.Severity);
            Assert.Equal(ruleRequest.Enabled, ruleResponse.Enabled);
            Assert.Equal(ruleRequest.Calculation, ruleResponse.Calculation);
            Assert.Equal(ruleRequest.Conditions[0].Field, ruleResponse.Conditions[0].Field);
            Assert.Equal(ruleRequest.Conditions[0].Operator, ruleResponse.Conditions[0].Operator);
            Assert.Equal(ruleRequest.Conditions[0].Value, ruleResponse.Conditions[0].Value);

            this.rulesCreated.Add(ruleResponse.Id); // Track new rule for deletion
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CreatesRuleWithAvg5MinCalculation_IfValid()
        {
            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Average", "300000");

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));

            var response = this.httpClient.PostAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ruleRequest.Name, ruleResponse.Name);
            Assert.Equal(ruleRequest.Description, ruleResponse.Description);
            Assert.Equal(ruleRequest.GroupId, ruleResponse.GroupId);
            Assert.Equal(ruleRequest.Severity, ruleResponse.Severity);
            Assert.Equal(ruleRequest.Enabled, ruleResponse.Enabled);
            Assert.Equal(ruleRequest.Calculation, ruleResponse.Calculation);
            Assert.Equal(ruleRequest.Conditions[0].Field, ruleResponse.Conditions[0].Field);
            Assert.Equal(ruleRequest.Conditions[0].Operator, ruleResponse.Conditions[0].Operator);
            Assert.Equal(ruleRequest.Conditions[0].Value, ruleResponse.Conditions[0].Value);

            this.rulesCreated.Add(ruleResponse.Id); // Track new rule for deletion
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CreatesRuleWithAvg10MinCalculation_IfValid()
        {
            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Average", "600000");

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));

            var response = this.httpClient.PostAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ruleRequest.Name, ruleResponse.Name);
            Assert.Equal(ruleRequest.Description, ruleResponse.Description);
            Assert.Equal(ruleRequest.GroupId, ruleResponse.GroupId);
            Assert.Equal(ruleRequest.Severity, ruleResponse.Severity);
            Assert.Equal(ruleRequest.Enabled, ruleResponse.Enabled);
            Assert.Equal(ruleRequest.Calculation, ruleResponse.Calculation);
            Assert.Equal(ruleRequest.Conditions[0].Field, ruleResponse.Conditions[0].Field);
            Assert.Equal(ruleRequest.Conditions[0].Operator, ruleResponse.Conditions[0].Operator);
            Assert.Equal(ruleRequest.Conditions[0].Value, ruleResponse.Conditions[0].Value);

            this.rulesCreated.Add(ruleResponse.Id); // Track new rule for deletion
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void PutCreatesRuleWithId_IfValid()
        {
            string ruleId = "TESTRULEID" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Average", "600000");

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + ruleId);
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));

            var response = this.httpClient.PutAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ruleRequest.Name, ruleResponse.Name);
            Assert.Equal(ruleRequest.Description, ruleResponse.Description);
            Assert.Equal(ruleRequest.GroupId, ruleResponse.GroupId);
            Assert.Equal(ruleRequest.Severity, ruleResponse.Severity);
            Assert.Equal(ruleRequest.Enabled, ruleResponse.Enabled);
            Assert.Equal(ruleRequest.Calculation, ruleResponse.Calculation);
            Assert.Equal(ruleRequest.Conditions[0].Field, ruleResponse.Conditions[0].Field);
            Assert.Equal(ruleRequest.Conditions[0].Operator, ruleResponse.Conditions[0].Operator);
            Assert.Equal(ruleRequest.Conditions[0].Value, ruleResponse.Conditions[0].Value);

            this.rulesCreated.Add(ruleResponse.Id); // Track new rule for deletion
        }

        private RuleApiModel GetSampleRuleWithCalculation(string calculation, string timePeriod)
        {
            var condition = new ConditionApiModel()
            {
                Field = "pressure",
                Operator = "GreaterThan",
                Value = "150"
            };

            var conditions = new List<ConditionApiModel> { condition };

            return new RuleApiModel()
            {
                Name = "Instant Rule",
                Description = "Instant Description",
                GroupId = DEFAULT_CHILLERS_GROUP_ID,
                Severity = "Info",
                Enabled = true,
                Calculation = calculation,
                TimePeriod = timePeriod,
                Conditions = conditions
            };
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
                var chillerRequest = new HttpRequest(Constants.CONFIG_ADDRESS + "/devicegroups/" + DEFAULT_CHILLERS_GROUP_ID);
                chillerRequest.AddHeader("X-Foo", "Bar");

                var response = this.httpClient.GetAsync(chillerRequest).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                // wait 10 seconds before retry if able
                if (i < SEED_DATA_RETRY_COUNT - 1) System.Threading.Thread.Sleep(SEED_DATA_RETRY_MSEC);
            }

            return false;
        }
    }
}
