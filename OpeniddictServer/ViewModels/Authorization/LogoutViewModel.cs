using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OpeniddictServer.ViewModels.Authorization
{
    public class LogoutViewModel
    {
        [BindNever]
        public string RequestId { get; set; }
    }
}
