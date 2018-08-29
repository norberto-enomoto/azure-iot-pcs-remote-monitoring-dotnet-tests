// Copyright (c) Microsoft. All rights reserved.

using Helpers.Http;
using Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Xunit;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    [Collection("IoTHub Manager Tests")]
    public class DeploymentsTests : IDisposable
    {
        private readonly IHttpClient httpClient;
        private const int TIMEOUT_MS = 10000;
        private const string PKG_TYPE_PARAM_NAME = "type";
        private const string PACKAGE_PARAMETER_NAME = "package";

        private const string TEST_PACKAGE_JSON =
                @"{
                    ""id"": ""tempid"",
                    ""schemaVersion"": ""1.0"",
                    ""content"": {
                        ""modulesContent"": {
                        ""$edgeAgent"": {
                            ""properties.desired"": {
                            ""schemaVersion"": ""1.0"",
                            ""runtime"": {
                                ""type"": ""docker"",
                                ""settings"": {
                                ""loggingOptions"": """",
                                ""minDockerVersion"": ""v1.25""
                                }
                            },
                            ""systemModules"": {
                                ""edgeAgent"": {
                                ""type"": ""docker"",
                                ""settings"": {
                                    ""image"": ""mcr.microsoft.com/azureiotedge-agent:1.0"",
                                    ""createOptions"": ""{}""
                                }
                                },
                                ""edgeHub"": {
                                ""type"": ""docker"",
                                ""settings"": {
                                    ""image"": ""mcr.microsoft.com/azureiotedge-hub:1.0"",
                                    ""createOptions"": ""{}""
                                },
                                ""status"": ""running"",
                                ""restartPolicy"": ""always""
                                }
                            },
                            ""modules"": {}
                            }
                        },
                        ""$edgeHub"": {
                            ""properties.desired"": {
                            ""schemaVersion"": ""1.0"",
                            ""routes"": {
                                ""route"": ""FROM /messages/* INTO $upstream""
                            },
                            ""storeAndForwardConfiguration"": {
                                ""timeToLiveSecs"": 7200
                            }
                            }
                        }
                        }
                    },
                    ""targetCondition"": ""*"",
                    ""priority"": 30,
                    ""labels"": {
                        ""Name"": ""Test""
                    },
                    ""createdTimeUtc"": ""2018-08-20T18:05:55.482Z"",
                    ""lastUpdatedTimeUtc"": ""2018-08-20T18:05:55.482Z"",
                    ""etag"": null,
                    ""metrics"": {
                        ""results"": {},
                        ""queries"": {}
                    }
                 }";

        private string packageId;
        private string deviceGroupId;
        private string deploymentId;
        private HttpRequestWrapper Request;

        List<string> disposeList = new List<string>();
        
        public DeploymentsTests()
        {
            this.httpClient = new Helpers.Http.HttpClient();
            this.packageId = this.CreatePackageAndGetId().ToLower();
            this.deviceGroupId = this.GetDependentDeviceGroupId().ToLower();
            this.deploymentId = $"{this.deviceGroupId}--{this.packageId}".ToLower();
            this.Request = new HttpRequestWrapper(Constants.IOT_HUB_ADDRESS, Constants.Urls.DEPLOYMENTS_PATH);
        }

        // Create deployment
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldCreateDeployment()
        {
            var deploymentName = "depName";
            var priority = 10;
            var response = this.CreateDeployment(deploymentName, this.deviceGroupId, packageId, priority);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var deployment = JsonConvert.DeserializeObject<DeploymentApiModel>(response.Content);
            Assert.Equal(deploymentName, deployment.Name);
            Assert.Equal(this.deploymentId, deployment.Id);
            Assert.Equal(this.deviceGroupId, deployment.DeviceGroupId);
            Assert.Equal(this.packageId, deployment.PackageId);
            Assert.Equal(priority, deployment.Priority);

            var elapsed = DateTime.UtcNow - deployment.CreatedDateTimeUtc;
            Assert.True(elapsed.TotalSeconds <= 3);

            this.DeleteDeployment(deploymentId);
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldRetrieveCreatedDeployment()
        {
            var deploymentName = "test-retrieve-deployment";
            var response = this.CreateDeployment(deploymentName, this.deviceGroupId, this.packageId, 10);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var getResponse = this.Request.Get(this.deploymentId, string.Empty);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var deployment = JsonConvert.DeserializeObject<DeploymentApiModel>(getResponse.Content);
            Assert.Equal(deploymentName, deployment.Name);
            Assert.Equal(this.deploymentId, deployment.Id);
            Assert.Equal(this.deviceGroupId, deployment.DeviceGroupId);
            Assert.Equal(this.packageId, deployment.PackageId);
            Assert.Equal(10, deployment.Priority);

            var elapsed = DateTime.UtcNow - deployment.CreatedDateTimeUtc;
            Assert.True(elapsed.TotalSeconds <= 5);

            this.DeleteDeployment(deploymentId);
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldRetrieveAllCreatedDeployments()
        {
            // Arrange, create 3 packages and deployments of each
            HashSet<string> packagesCreated = new HashSet<string>();
            var deploymentName = "test-retrieve-all-deployments";
            var numDeployments = 3;
            for(int i = 0; i < numDeployments; i++) 
            {
                var pkgId = CreatePackageAndGetId();
                packagesCreated.Add(pkgId);
                var response = this.CreateDeployment(deploymentName + i, this.deviceGroupId, pkgId, 10);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            // Act retrieve all
            var getResponse = this.Request.Get();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            // Assert each package id created has a corresponding deployment and no extras.
            var deployments = JsonConvert.DeserializeObject<DeploymentListApiModel>(getResponse.Content);
            foreach(DeploymentApiModel deployment in deployments.Items)
            {
                if(!deployment.Name.StartsWith(deploymentName))
                {
                    continue;
                }

                Assert.True(packagesCreated.Remove(deployment.PackageId), "Returned a package id that was not created by this test");
                this.DeletePackage(deployment.PackageId);
                this.DeleteDeployment(deployment.Id);

                Assert.Equal(this.deviceGroupId, deployment.DeviceGroupId);
            }

            Assert.True(packagesCreated.Count == 0, "Missing deployments for some created packages");
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldDeleteDeployment()
        {
            // Arrange - Create package to be deleted, and verify it exists
            var deploymentName = "test-delete-deployment";
            var response = this.CreateDeployment(deploymentName, this.deviceGroupId, this.packageId, 10);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var getResponse = this.Request.Get(this.deploymentId, string.Empty);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            // Act - Delete package
            var deleteResponse = this.DeleteDeployment(this.deploymentId);
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            // Assert - Verify package no longer exists
            getResponse = this.Request.Get(this.deploymentId, string.Empty);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        private IHttpResponse CreateDeployment(string name, string deviceGroupId, string packageId, int priority)
        {
            var input = new DeploymentApiModel
            {
                Name = name,
                DeviceGroupId = deviceGroupId,
                PackageId = packageId,
                Priority = priority,
                Type = DeploymentType.EdgeManifest
            };

            return this.Request.Post(JsonConvert.SerializeObject(input, Formatting.None));
        }

        private IHttpResponse DeleteDeployment(string deploymentId)
        {
            return this.Request.Delete(deploymentId);
        }

        private string CreatePackageAndGetId()
        {
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/packages");
            request.Options.Timeout = TIMEOUT_MS;

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("EdgeManifest"), PKG_TYPE_PARAM_NAME);

            var jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(TEST_PACKAGE_JSON);
            ByteArrayContent bytes = new ByteArrayContent(jsonAsBytes);
            content.Add(bytes, PACKAGE_PARAMETER_NAME, "default package");

            request.SetContent(content);
            
            var response = this.httpClient.PostAsync(request).Result;
            var parsedResponse = JToken.Parse(response.Content);
            return parsedResponse["Id"].ToString();
        }

        private void DeletePackage(string packageId)
        {
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/packages/" + packageId);
            request.Options.Timeout = TIMEOUT_MS;
            this.httpClient.DeleteAsync(request);
        }

        private string GetDependentDeviceGroupId()
        {
            var requestAddress = Constants.CONFIG_ADDRESS + "/devicegroups";
            var response = HttpHelpers.SendHttpGetRequestWithRetry(requestAddress, this.httpClient);

            // Assert
            Assert.NotNull(response);
            var jsonResponse = HttpHelpers.GetJsonResponseIfValid(response);
            var items = (JArray)jsonResponse["items"];
            Assert.True(items.Count >= 1);

            return items.First()["Id"].ToString();
        }

        public void Dispose()
        {
            this.DeletePackage(this.packageId);
        }
    }
}
