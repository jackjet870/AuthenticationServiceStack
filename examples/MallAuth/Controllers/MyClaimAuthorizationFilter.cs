using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace MallAuth.Controllers
{
    class MyClaimAuthorizationFilterAttribute : AuthServiceStack.AuthProvider.TransClaimsAuthorizationFilter
    {
        public MyClaimAuthorizationFilterAttribute(string claimType = null,string claimValue = null)
        {
            this.ClaimType = claimType;
            this.ClaimValue = claimValue;
            this.ClaimsAbbreviationDictionary = () => Shared.MallClaimsDefinition.dict;

            //new AuthServiceStack.AuthProvider.getClaimsTrans(getClaimsTrans);
            //this.getUserClaims = ServerCache.GlobalCache.getInstance().getUserClaims
        }
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}