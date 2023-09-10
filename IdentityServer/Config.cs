﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.s
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources() =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> Apis() =>
            new List<ApiResource>
            {
                new ApiResource("ApiName")
                {
                    Scopes = new List<string>{ "Api.read", "Api.write" },
                    ApiSecrets = { new Secret("secret_for_the_api".Sha256()) }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope("ApiName", "My API")
            };

        public static IEnumerable<Client> Clients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "ConsoleApp_ClientId",
                    ClientSecrets = { new Secret("secret_for_the_consoleapp".Sha256()) },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "Api.read" },
                }
            };

        public static List<TestUser> TestUsers() =>
            new()
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "namjiAdmin",
                    Password = "W1nt3rL4k399", //.Sha265() removed because it was confusing withouth adding any value, https://github.com/georgekosmidis/IdentityServer4.SetupSample/issues/1
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Namji Admin"),
                        new Claim(JwtClaimTypes.GivenName, "Namji"),
                        new Claim(JwtClaimTypes.FamilyName, "Admin"),
                        new Claim(JwtClaimTypes.WebSite, "https://namji.uk/"),
                    }
                }
            };
    }
}