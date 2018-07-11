// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using Helpers;
using Helpers.Http;
using Helpers.Models;
using Newtonsoft.Json;
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

        public RulesTest()
        {
            this.httpClient = new HttpClient();
            this.rulesCreated = new List<string>();
        }

        ~RulesTest()
        {
            Console.WriteLine("Rules test cleanup: Deleting " + this.rulesCreated.Count + " rules.");

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
        public void GetRuleById_ReturnsRule()
        {
            string ruleId = "TESTRULEID" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Average", "600000");

            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));

            var newRuleResponse = this.httpClient.PostAsync(request).Result;
            var newRule = JsonConvert.DeserializeObject<RuleApiModel>(newRuleResponse.Content);

            // Act
            request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + newRule.Id);
            request.AddHeader("X-Foo", "Bar");

            var response = this.httpClient.GetAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(newRule.Id, ruleResponse.Id);
            Assert.Equal(newRule.Name, ruleResponse.Name);
            Assert.Equal(newRule.Description, ruleResponse.Description);
            Assert.Equal(newRule.GroupId, ruleResponse.GroupId);
            Assert.Equal(newRule.Severity, ruleResponse.Severity);
            Assert.Equal(newRule.Enabled, ruleResponse.Enabled);
            Assert.Equal(newRule.Calculation, ruleResponse.Calculation);
            Assert.Equal(newRule.Conditions[0].Field, ruleResponse.Conditions[0].Field);
            Assert.Equal(newRule.Conditions[0].Operator, ruleResponse.Conditions[0].Operator);
            Assert.Equal(newRule.Conditions[0].Value, ruleResponse.Conditions[0].Value);
        }

        // TODO This test fails due to a discovered bug in Telemetry PUT request.
        //      Fix PUT with an ID to create a rule with that ID and uncomment.
        /*
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void PutCreatesRuleWithId_IfValid()
        {
            string newRuleId = "TESTRULEID" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Average", "600000");

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + newRuleId);
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));

            var response = this.httpClient.PutAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(newRuleId, ruleResponse.Id);
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
        */

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void PutUpdatesExistingRuleToDisabled_IfValid()
        {
            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var newRuleRequest = this.GetSampleRuleWithCalculation("Average", "600000");

            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(newRuleRequest));

            var newRuleResponse = this.httpClient.PostAsync(request).Result;
            var newRule = JsonConvert.DeserializeObject<RuleApiModel>(newRuleResponse.Content);

            // Act
            newRule.Enabled = false;

            request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + newRule.Id);
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(newRule));

            var updateResponse = this.httpClient.PutAsync(request).Result;
            var updatedRule = JsonConvert.DeserializeObject<RuleApiModel>(updateResponse.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Equal(newRule.Enabled, updatedRule.Enabled);

            this.rulesCreated.Add(updatedRule.Id); // Track new rule for deletion
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeleteRuleReturnsOK_IfRuleExists()
        {
            string ruleId = "TESTRULEID" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Arrange  
            Assert.True(this.ValidChillerGroup()); // Make sure device groups have been created by seed data
            var ruleRequest = this.GetSampleRuleWithCalculation("Average", "600000");

            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + ruleId);
            request.AddHeader("X-Foo", "Bar");
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(ruleRequest));

            var response = this.httpClient.PutAsync(request).Result;

            // Act
            request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + ruleId);
            request.AddHeader("X-Foo", "Bar");

            response = this.httpClient.DeleteAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
                Name = calculation + " Test Rule",
                Description = "Test Description",
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
