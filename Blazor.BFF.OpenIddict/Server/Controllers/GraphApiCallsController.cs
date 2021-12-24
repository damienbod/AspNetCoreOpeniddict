using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.BFF.OpenIddict.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace Blazor.BFF.OpenIddict.Server.Controllers
{
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [AuthorizeForScopes(Scopes = new string[] { "User.ReadBasic.All user.read" })]
    [ApiController]
    [Route("api/[controller]")]
    public class GraphApiCallsController : ControllerBase
    {
        private GraphApiClientService _graphApiClientService;

        public GraphApiCallsController(GraphApiClientService graphApiClientService)
        {
            _graphApiClientService = graphApiClientService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var userData = await _graphApiClientService.GetGraphApiUser();
            return new List<string> { $"DisplayName: {userData.DisplayName}",
                $"GivenName: {userData.GivenName}", $"AboutMe: {userData.AboutMe}" };
        }
    }
}
