# IdentityServer4 Port &amp; playground
## preamble

IdentityServer4 has been a popular open source OAuth 2.0 and OpenID Connect framework for ASP.NET Core for several years. With the release of .NET 7 and ASP.NET Core 7, it is time to update IdentityServer4 to take advantage of the latest features and improvements in the .NET ecosystem.

This project ports IdentityServer4 to .NET 7 and ASP.NET Core 7 to allow existing IdentityServer4 users to upgrade and benefit from performance enhancements, new APIs, and other improvements in the latest .NET release. Updating IdentityServer4 to .NET 7 will also ensure continued support and compatibility as .NET 6 approaches end-of-life in the coming years.

Key goals for this port include taking advantage of .NET 7 performance optimizations like improved garbage collection and runtime throughput. We also aim to leverage new ASP.NET Core 7 features like minimal APIs to help streamline IdentityServer4's implementation.

This solution will also provide a quickstart point for anyone needing a .NET 7 Token Service that can be integrated very quickly with minimal requirements and code (hence the move to minimal API).

## Limitations
The Razor pages have yet to ported over however there are already a number of well develop IdentityServer4 admin interfaces such as [Skoruba.IdentityServer4.Admin](https://github.com/skoruba/IdentityServer4.Admin) which is fairly mature based on ASP.NET MVC with .NET 6

## Further reading
### IdentityServer4 setup
You can view the IdentityServer4 guide [here](https://github.com/TriadGroupPlc/IdentityServer4Port/blob/master/IdentityServer/README.md)
