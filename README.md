[![.NET](https://github.com/damienbod/AspNetCoreOpeniddict/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AspNetCoreOpeniddict/actions/workflows/dotnet.yml)

# Implementing OpenID Code Flow PKCE using OpenIddict and Angular

## Blog: 

https://damienbod.com/2017/04/11/implementing-openid-implicit-flow-using-openiddict-and-angular/

## Creating Migrations

### Console

dotnet ef migrations add initSts -c ApplicationDbContext

### Powershell

Add-Migration "init_sts" -c ApplicationDbContext  

## Running manually

Update-Database -Context ApplicationDbContext

## History

2021-12-24 Updating .NET 6

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
