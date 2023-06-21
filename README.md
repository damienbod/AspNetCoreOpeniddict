[![.NET](https://github.com/damienbod/AspNetCoreOpeniddict/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AspNetCoreOpeniddict/actions/workflows/dotnet.yml)

# Implementing OpenID Code Flow PKCE using OpenIddict, Angular and Blazor

- Angular Code flow PKCE public client
- Blazor WASM, ASP.NET Core hosted BFF Code flow PKCE, trusted client
- API OAUTH2 introspection and reference tokens
- OpenID Connect server implemented with OpenIddict

## Blogs: 

[Implementing OpenID Code Flow with PKCE using OpenIddict and Angular](https://damienbod.com/2017/04/11/implementing-openid-implicit-flow-using-openiddict-and-angular/)

[Secure a Blazor WASM ASP.NET Core hosted APP using BFF and OpenIddict](https://damienbod.com/2022/01/03/secure-a-blazor-wasm-asp-net-core-hosted-app-using-bff-and-openiddict/)

[Using Blazor with a YARP downstream API protected using certificate authentication](https://damienbod.com/2022/02/07/using-blazor-with-a-yarp-downstream-api-protected-using-certificate-authentication/)

[Implement an OpenIddict identity provider using ASP.NET Core Identity with Keycloak federation](https://damienbod.com/2022/05/02/implement-an-openiddict-identity-provider-using-asp-net-core-identity-with-keycloak-federation/)

[Add Fido2 MFA to an OpenIddict identity provider using ASP.NET Core Identity](https://damienbod.com/2022/07/04/add-fido2-mfa-to-an-openiddict-identity-provider-using-asp-net-core-identity/)

[Implement a GRPC API with OpenIddict and the OAuth client credentials flow](https://damienbod.com/2022/09/05/implement-a-grpc-api-with-openiddict-and-oauth-the-client-credentials-flow/)

## Creating Migrations

### Console

dotnet ef migrations add initSts -c ApplicationDbContext

### Powershell

Add-Migration "init_sts" -c ApplicationDbContext  

## Running manually

Update-Database -Context ApplicationDbContext

## History

2023-08-20 Updated packages

2023-04-28 Updated packages

2023-01-10 Updated packages

2022-12-24 Updated .NET 7, OpenIddict v4

2022-09-02 Updated packages, added client credentials flows

2022-08-20 Updated packages

2022-07-01 Add support for FIDO2 to the OpenIddict server

2022-06-19 Update packages

2022-06-04 Update packages

2022-05-20 Update packages

2022-05-02 Update packages, add external provider

2022-03-20 Update , using nullable

2022-02-07 Update solution

2022-01-23 Fix API calls

2022-01-21 Update project

2022-01-04 Update STS 

2022-01-01 Added Blazor BFF WASM ASP.NET Core hosted demo

2021-12-24 Updating .NET 6, Angular 13.1.0

2021-07-01 Updating .NET 5, Angular 12

2021-04-21 Updated packages, improved scope validation

2020-12-26 Updating to .NET 5, Angular 11

2018-05-27 Updating to .NET Core 2.1

2018-02-09 Updating npm and Angular 5.2.4

2018-02-03 Updating npm and nuget packages, Angular 5.2.3, angular-auth-oidc-client 4.0.1

2017-11-24 Updated ASP.NET Core 2, Angular 5.0.3, angular-auth-oidc-client

2017-06-13 Updated using angular-auth-oidc-client 0.0.4 and Angular to 4.2.2

2017.06.09 Updated Auth Module, and angular to 4.1.3

## Links:

https://documentation.openiddict.com/

https://github.com/damienbod/Blazor.BFF.OpenIDConnect.Template

https://github.com/openiddict/openiddict-core

https://github.com/robinvanderknaap/authorization-server-openiddict

http://kevinchalet.com/2016/07/13/creating-your-own-openid-connect-server-with-asos-implementing-the-authorization-code-and-implicit-flows/

https://github.com/openiddict/openiddict-core/issues/49

https://github.com/openiddict/openiddict-samples

https://blogs.msdn.microsoft.com/webdev/2017/01/23/asp-net-core-authentication-with-identityserver4/

https://blogs.msdn.microsoft.com/webdev/2016/10/27/bearer-token-authentication-in-asp-net-core/

https://blogs.msdn.microsoft.com/webdev/2017/04/06/jwt-validation-and-authorization-in-asp-net-core/

https://jwt.io/

https://www.scottbrady91.com/OpenID-Connect/OpenID-Connect-Flows

https://damienbod.com/2022/01/10/comparing-the-backend-for-frontend-bff-security-architecture-with-an-spa-ui-using-a-public-api/

## Keycloak links

https://docs.microsoft.com/en-us/java/openjdk/download

>https://github.com/tuxiem/AspNetCore-keycloak

https://wjw465150.gitbooks.io/keycloak-documentation/content/server_installation/topics/network/https.html
