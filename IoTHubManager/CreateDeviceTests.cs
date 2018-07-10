// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using System;
using System.Security.Cryptography;
using System.Text;
using Helpers.Http;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    public class CreateDeviceTest
    {

        internal HttpRequestWrapper Request;

        private string DEVICE_TEMPLATE_AUTO_GEN_AUTH, 
                       DEVICE_TEMPLATE_SYMMETRIC_AUTH,
                       DEVICE_TEMPLATE_X509_AUTH;

        public CreateDeviceTest()
        {
            //Create request wrapper object for interacting with iothub manager microservices for CRUD on devices
            this.Request = new HttpRequestWrapper(Constants.Urls.IOTHUB_ADDRESS, Constants.Urls.DEVICE_PATH);

            //Fetch different device templates
            DEVICE_TEMPLATE_AUTO_GEN_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_AUTO_GEN_AUTH);
            DEVICE_TEMPLATE_SYMMETRIC_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_SYMMETRIC_AUTH);
            DEVICE_TEMPLATE_X509_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_X509_AUTH);
                                 
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Tests for creation of devices with all permutation of AUTH and Id creation
        /// </summary>
        

        /**
         * Creates a device with auto generated id and symmetric auth.
         * Verifies that the resultant device has system-generated
         * id as well as auth credentails.
         */
        [Fact, Trait(Constants.TEST , Constants.INTEGRATION_TEST )]
        public void Creates_Device_with_AutoGenerated_Id_and_Auth()
        {
            // DeviceId must be empty to be auto generated.
            var device = DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, "");
            var response = Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString(),
                   primaryKey = authentication["PrimaryKey"].ToString(),
                   secondaryKey = authentication["SecondaryKey"].ToString();

            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.False(string.IsNullOrEmpty(createdDeviceId));
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.False(string.IsNullOrEmpty(primaryKey));
            Assert.False(string.IsNullOrEmpty(secondaryKey));
            
        }

        /**
         * Creates a device with Custom id and auto generated auth.
         * Verifies that the resultant device has correct id and system
         * generated symmetric auth credentails.
         */
        [Fact, Trait(Constants.TEST , Constants.INTEGRATION_TEST )]
        public void Creates_Device_with_Custom_Id_and_AutoGenerated_Auth()
        {
            string id = Guid.NewGuid().ToString();
            var device = DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, id);
            var response = Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString(),
                   primaryKey = authentication["PrimaryKey"].ToString(),
                   secondaryKey = authentication["SecondaryKey"].ToString();

            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.Equal(createdDeviceId, id);
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.False(string.IsNullOrEmpty(primaryKey));
            Assert.False(string.IsNullOrEmpty(secondaryKey));
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
        }

        /**
         * Creates a device with custom id and symmetric auth
         * credentails. Verifies that the resultant device has 
         * correct id and auth credentails.
         */
        [Fact, Trait(Constants.TEST , Constants.INTEGRATION_TEST )]
        public void Creates_Device_with_Custom_Id_and_Auth()
        {

            string id = Guid.NewGuid().ToString(),
                   primaryKey = Guid.NewGuid().ToString("n"),
                   secondaryKey = Guid.NewGuid().ToString("n");

            string device = DEVICE_TEMPLATE_SYMMETRIC_AUTH.Replace(Constants.Keys.DEVICE_ID, id)
                                                          .Replace(Constants.Keys.PRIMARY_KEY, primaryKey)
                                                          .Replace(Constants.Keys.SECONDARY_KEY, secondaryKey);
            var response = Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString(),
                   createdPrimaryKey = authentication["PrimaryKey"].ToString(),
                   createdSecondaryKey = authentication["SecondaryKey"].ToString();

            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.Equal(createdDeviceId, id);
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.Equal(primaryKey, createdPrimaryKey);
            Assert.Equal(secondaryKey, createdSecondaryKey);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
        }

        /**
         * Creates a device with auto generated id and custom 
         * symmetric auth credentials. Verifies that the resultant device 
         * has system-generated id and correct auth credentails.
         */
        [Fact, Trait(Constants.TEST , Constants.INTEGRATION_TEST )]
        public void Creates_Device_with_AutoGen_Id_and_Custom_Auth()
        {
            // DeviceId must be empty to be auto generated.
            string primaryKey = Guid.NewGuid().ToString("N"),
                   secondaryKey = Guid.NewGuid().ToString("N");

            string device = DEVICE_TEMPLATE_SYMMETRIC_AUTH.Replace(Constants.Keys.DEVICE_ID, "")
                                                          .Replace(Constants.Keys.PRIMARY_KEY, primaryKey)
                                                          .Replace(Constants.Keys.SECONDARY_KEY, secondaryKey);
            var response = Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString(),
                   createdPrimaryKey = authentication["PrimaryKey"].ToString(),
                   createdSecondaryKey = authentication["SecondaryKey"].ToString();

            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.False(string.IsNullOrEmpty(createdDeviceId));
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.Equal(primaryKey, createdPrimaryKey);
            Assert.Equal(secondaryKey, createdSecondaryKey);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
        }

        /**
         * Creates a device with custom id and X509 auth credentails.
         * Verifies that the resultant device has correct id
         * and auth credentails.
         */
        [Fact, Trait(Constants.TEST , Constants.INTEGRATION_TEST )]
        public void Creates_Device_with_Custom_Id_and_X509_Auth()
        {

            string id = Guid.NewGuid().ToString(),
                   primaryThumbprint = generateNewThumbPrint(),
                   secondaryThumbprint = generateNewThumbPrint();

            string device = DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, id)
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);
            var response = Request.Post(device);

            // Asserts 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString(),
                   createdPrimaryThumbprint = authentication["PrimaryThumbprint"].ToString(),
                   createdSecondaryThumbprint = authentication["SecondaryThumbprint"].ToString();

            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.Equal(createdDeviceId, id);
            Assert.Equal(Constants.Auth.X509, authentication["AuthenticationType"]);
            Assert.Equal(primaryThumbprint, createdPrimaryThumbprint);
            Assert.Equal(secondaryThumbprint, createdSecondaryThumbprint);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
            
        }

        /**
         * Creates a device with auto generated id and custom 
         * X509 auth credentials. Verifies that the resultant device 
         * has system-generated id and correct auth credentails.
         */
        [Fact, Trait(Constants.TEST , Constants.INTEGRATION_TEST )]
        public void Creates_Device_AutoGen_Id_and_Custom_X509_Auth()
        {
            // DeviceId must be empty to be auto generated.
            string primaryThumbprint = generateNewThumbPrint(),
                   secondaryThumbprint = generateNewThumbPrint();
           
            string device = DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, "")
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);
            var response = Request.Post(device);
            
            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString(),
                   createdPrimaryThumbprint = authentication["PrimaryThumbprint"].ToString(),
                   createdSecondaryThumbprint = authentication["SecondaryThumbprint"].ToString();

            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.False(string.IsNullOrEmpty(createdDeviceId));
            Assert.Equal(Constants.Auth.X509, authentication["AuthenticationType"]);
            Assert.Equal(primaryThumbprint, createdPrimaryThumbprint);
            Assert.Equal(secondaryThumbprint, createdSecondaryThumbprint);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
        }

        /*
        Generates random SHA1 hash mimicing the X509 thumb print
         */
        private string generateNewThumbPrint(){
           
            string input = Guid.NewGuid().ToString();
            SHA1Managed sha = new SHA1Managed();

            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var stringBuilder = new StringBuilder(hash.Length * 2);

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}