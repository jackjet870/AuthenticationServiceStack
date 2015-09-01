using System.Collections.Generic;
using System.Security.Claims;

namespace MallAuth.DAL
{
    public class AppAuthRepository : AuthServiceStack.AuthModel.EF.AuthWithClaimsRepository
    {
        public AppAuthRepository() :
            base(new AppAuthContext(),Shared.MallClaimsDefinition.dict)
        {
        }

        
        public override List<Claim> getUserClaims(string username)
        {  
            //todo 
            //make up your user-claims such as" this._ctx.CustomUserClaims.."
            return base.getUserClaims(username);
        }
    }
}