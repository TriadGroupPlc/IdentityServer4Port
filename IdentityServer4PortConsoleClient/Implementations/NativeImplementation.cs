﻿using IdentityModel;
using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IdentityServerTest.ConsoleApp.Implementations
{
    public static class NativeImplementation
    {
        public static async Task Sample(string username, string password)
        {
            var httpClient = new HttpClient();

            //Just a sample call with an invalid access token.
            // The expected response from this call is 401 Unauthorized
            var apiResponse = await httpClient.GetAsync("https://localhost:44328/helloworld");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid_access_token");

            //The API is protected, let's ask the user for credentials and exchanged them with an access token
            if (apiResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //Make the call and get the result
                var identityServerResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = "http://localhost:5000/connect/token",

                    // @Doc-Saintly, 2020-06-28: I think this line is unnecessary because RequestPasswordTokenAsync should automatically set the grant type
                    // Issue: https://github.com/georgekosmidis/IdentityServer4.SetupSample/issues/1
                    // GrantType = "password",

                    ClientId = "83A0DD78191B4C3F8F932E27E95852E6",
                    ClientSecret = "15F47BBA443B42D9B5AFA06648477F86",
                    Scope = "IdentityServer4PortClientApi",

                    UserName = username,
                    Password = password //.Sha265() removed because it was confusing withouth adding any value, https://github.com/georgekosmidis/IdentityServer4.SetupSample/issues/1
                });

                //all good?
                if (!identityServerResponse.IsError)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    Console.WriteLine("SUCCESS!!");
                    Console.WriteLine();
                    Console.WriteLine("Access Token: ");
                    Console.WriteLine(identityServerResponse.AccessToken);

                    //Call the API with the correct access token
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", identityServerResponse.AccessToken);
                    apiResponse = await httpClient.GetAsync("https://localhost:44328/helloworld");

                    Console.WriteLine();
                    Console.WriteLine("API response:");
                    Console.WriteLine(await apiResponse.Content.ReadAsStringAsync());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("Failed to login with error:");
                    Console.WriteLine(identityServerResponse.ErrorDescription);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("YOU ARE NOT PROTECTED!!!");
            }
        }
    }
}
