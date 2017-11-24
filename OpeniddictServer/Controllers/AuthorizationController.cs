/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using OpeniddictServer.Models;
using OpeniddictServer.ViewModels.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Core;
using System.Security.Claims;

namespace OpeniddictServer.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("~/connect/authorize")]
        public async Task<IActionResult> Authorize(OpenIdConnectRequest request)
        {
            Debug.Assert(request.IsAuthorizationRequest(),
                "The OpenIddict binder for ASP.NET Core MVC is not registered. " +
                "Make sure services.AddOpenIddict().AddMvcBinders() is correctly called.");

            if (!User.Identity.IsAuthenticated)
            {
                // If the client application request promptless authentication,
                // return an error indicating that the user is not logged in.
                if (request.HasPrompt(OpenIdConnectConstants.Prompts.None))
                {
                    var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIdConnectConstants.Properties.Error] = OpenIdConnectConstants.Errors.LoginRequired,
                        [OpenIdConnectConstants.Properties.ErrorDescription] = "The user is not logged in."
                    });

                    // Ask OpenIddict to return a login_required error to the client application.
                    return Forbid(properties, OpenIdConnectServerDefaults.AuthenticationScheme);
                }

                return Challenge();
            }

            // Retrieve the profile of the logged in user.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Error = OpenIdConnectConstants.Errors.ServerError,
                    ErrorDescription = "An internal error has occurred"
                });
            }

            // Create a new authentication ticket.
            var ticket = await CreateTicketAsync(request, user);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        [HttpGet("~/connect/logout")]
        public async Task<IActionResult> Logout()
        {
            // Ask ASP.NET Core Identity to delete the local and external cookies created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            await _signInManager.SignOutAsync();

            // Returning a SignOutResult will ask OpenIddict to redirect the user agent
            // to the post_logout_redirect_uri specified by the client application.
            return SignOut(OpenIdConnectServerDefaults.AuthenticationScheme);
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(OpenIdConnectRequest request, ApplicationUser user)
        {
            var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);

            // When access_token and id_token are specified,
            // the claim will be serialized in both tokens.
            ////identity.AddClaim("username", "Pinpoint",
            ////    OpenIdConnectConstants.Destinations.AccessToken,
            ////    OpenIdConnectConstants.Destinations.IdentityToken);

            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            foreach (var claim in principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                var destinations = new List<string>
                {
                    OpenIdConnectConstants.Destinations.AccessToken
                };

                if ((claim.Type == OpenIdConnectConstants.Claims.Name) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Email) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Role)  )
                {
                    destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);
                }

                claim.SetDestinations(destinations);

                identity.AddClaim(claim);
            }

            // Add custom claims
            var claimdataEventRecordsAdmin = new Claim("role", "dataEventRecords.admin");
            claimdataEventRecordsAdmin.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken);

            var claimAdmin = new Claim("role", "admin");
            claimAdmin.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken);

            var claimUser = new Claim("role", "dataEventRecords.user");
            claimUser.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken);

            identity.AddClaim(claimdataEventRecordsAdmin);
            identity.AddClaim(claimAdmin);
            identity.AddClaim(claimUser);

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity),
            new Microsoft.AspNetCore.Authentication.AuthenticationProperties(),
            OpenIdConnectServerDefaults.AuthenticationScheme);

            // Set the list of scopes granted to the client application.
            ticket.SetScopes(new[]
            {
                OpenIdConnectConstants.Scopes.OpenId,
                OpenIdConnectConstants.Scopes.Email,
                OpenIdConnectConstants.Scopes.Profile,
                "role",
                "dataEventRecords"
            }.Intersect(request.GetScopes()));

            ticket.SetResources("dataEventRecords");

            return ticket;
        }
    }
}