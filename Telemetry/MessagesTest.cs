﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using Helpers;
using Helpers.Http;
using Helpers.Models.TelemetryMessages;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Telemetry
{
    [Collection("Telemetry Tests")]
    public class MessagesTest
    {
        private readonly IHttpClient httpClient;
        private ITestOutputHelper logger;

        private const int MESSAGES_WAIT_MSEC = 30000;

        private const string MESSAGES_ENDPOINT_SUFFIX = "/messages";

        public MessagesTest(ITestOutputHelper logger)
        {
            this.httpClient = new HttpClient();
            this.logger = logger;

            // Wait for seed data to run simulation
            Assert.True(SeedData.WaitForSeedComplete());

            // Wait for messages to be generated by simulation
            System.Threading.Thread.Sleep(MESSAGES_WAIT_MSEC);
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void GetMessages_ReturnsList()
        {
            // Arrange  
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + MESSAGES_ENDPOINT_SUFFIX);

            // Act
            var response = this.httpClient.GetAsync(request).Result;
            var messageResponse = JsonConvert.DeserializeObject<MessageListApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(messageResponse.Items);
        }
    }
}
