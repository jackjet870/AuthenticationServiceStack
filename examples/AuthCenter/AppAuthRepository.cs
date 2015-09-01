using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthCenter
{
    public class AppAuthRepository : AuthServiceStack.AuthModel.EF.AuthWithClaimsRepository
    {
        public AppAuthRepository() :
            base(new AppAuthContext(), ClaimsAbbreviationDictionary.dict)
        {
        }


        public override List<System.Security.Claims.Claim> getUserClaims(string username)
        {
            //todo 
            //make up your user-claims such as" this._ctx.CustomUserClaims.."
            return base.getUserClaims(username);
        }
    }
}
