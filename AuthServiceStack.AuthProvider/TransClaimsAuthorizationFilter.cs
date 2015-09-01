using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AuthServiceStack.AuthProvider
{
    public delegate List<Claim> getUserClaims(string username);
    public delegate Dictionary<string, string> getClaimsTrans();

    public abstract class TransClaimsAuthorizationFilter : AuthorizationFilterAttribute
    {
        protected getClaimsTrans getClaimsTrans;
        //protected Dictionary<string, string> ClaimsTrans;
        protected string ClaimType { get; set; }
        protected string ClaimValue { get; set; }
        protected bool usingControllerAction {
            get
            {
                return string.IsNullOrWhiteSpace(this.ClaimType) || string.IsNullOrWhiteSpace(this.ClaimValue);
            }
        }

        //public TransClaimsAuthorizationFilter(Dictionary<string,string> ClaimsTrans = null, string ClaimType = null, string ClaimValue = null)
        //{
        //    this.ClaimType = ClaimType;
        //    this.ClaimValue = ClaimValue;
        //    this.ClaimsTrans = ClaimsTrans;
        //}
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            if (usingControllerAction) //缺省的,未指定Action 或 resource
            {
                var action = actionContext.ActionDescriptor.ActionName.ToLower();
                var resource = actionContext.ControllerContext.ControllerDescriptor.ControllerName.ToLower();
                try {
                    Dictionary<string, string> abbreviationDictionary = getClaimsTrans.Invoke();
                    ClaimType = abbreviationDictionary[action];
                }
                catch {
                    //raw claimtype without name-conflict
                    ClaimType = action;
                }
                ClaimValue = resource;
            }

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (!principal.Identity.IsAuthenticated)
            {
                return NotAllow(actionContext);

            }

            if (!(principal.HasClaim(x => x.Type == ClaimType && x.Value == ClaimValue)))
            {
                return NotAllow(actionContext);
            }

            //User is Authorized, complete execution
            return Task.FromResult<object>(null);

        }

        private static Task NotAllow(HttpActionContext actionContext)
        {
            var notAllowed = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response = notAllowed;
            return Task.FromResult<object>(null);
        }
    }
}