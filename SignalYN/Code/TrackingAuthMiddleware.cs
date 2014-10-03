using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace SignalYN
{
    public class TrackingAuthMiddleware : OwinMiddleware
    {
        public TrackingAuthMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            if (context.Authentication.User.Identity.IsAuthenticated == false)
            {
                var userName = Guid.NewGuid().ToBase64String();
                var identity = new ClaimsIdentity(new GenericIdentity(userName, "TrackingAuth"));
                context.Authentication.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true
                }, identity);
                context.Request.User = new ClaimsPrincipal(identity);
            }

            return this.Next.Invoke(context);
        }
    }
}