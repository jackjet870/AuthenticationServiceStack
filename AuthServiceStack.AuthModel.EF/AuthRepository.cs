using AuthServiceStack.AuthModel.EF;
using AuthServiceStack.AuthModel.EF.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace AuthServiceStack.AuthModel.EF
{

    public class AuthWithClaimsRepository : AuthRepository
    {

        public Dictionary<string, string> claimsAbbreviationDictionary { get; private set; }
        //public AuthWithClaimsRepository(string context, Dictionary<string, string> ClaimsTrans) 
        //    :base(context) {
        //    this.claimsTrans = ClaimsTrans;
        //}
        public AuthWithClaimsRepository(AuthContext context, Dictionary<string, string> claimsAbbreviationDictionary = null)
            : base(context)
        {
            this.claimsAbbreviationDictionary = claimsAbbreviationDictionary;
        }
        public virtual List<Claim> getUserClaims(string username)
        {

            var UserClaims_ = new List<Claim>();

            foreach (var uc in _ctx.CustomUserClaims.Where(s => s.UserReferId == username && s.valid == true))
            {

                if (null != claimsAbbreviationDictionary && claimsAbbreviationDictionary.ContainsKey(uc.ClaimType))
                    //short claim name for long-unique claim
                    UserClaims_.Add(new Claim(claimsAbbreviationDictionary[uc.ClaimType], uc.ClaimValue));
                else
                    //raw ClaimType without name comflict
                    UserClaims_.Add(new Claim(uc.ClaimType, uc.ClaimValue));

            }
            return UserClaims_;
        }
    }
    public class AuthRepository : IDisposable
    {
        protected AuthContext _ctx;

        private Microsoft.AspNet.Identity.UserManager<ApplicationUser> _userManager;

        public AuthRepository(string Context)
        {
            _ctx = new AuthContext(Context);
            _userManager = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }
        public AuthRepository(AuthContext Context)
        {
            _ctx = Context;
            _userManager = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }
        public async Task<Microsoft.AspNet.Identity.IdentityResult> RegisterUser(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }
        public Microsoft.AspNet.Identity.IdentityResult RegisterUser(string username, string password)
        {
            return RegisterUser(new UserModel { Password = password, UserName = username }).Result;
        }
        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);

            return user;
        }


        public void Dispose()
        {
            //_userManager.GenerateUserTokenAsync
            _ctx.Dispose();
            _userManager.Dispose();

        }


        public Client FindClient(string ClientId)
        {
            //return _ctx.Clients.FirstOrDefault(s => s.Id == ClientId);
            var client = _ctx.Clients.Find(ClientId);
            return client;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public async Task<bool> AddRefreshToken(RefreshToken rt)
        {
            var existsToken = _ctx.RefreshTokens.Where(s => s.ClientId == rt.ClientId && s.Subject == rt.Subject).SingleOrDefault();
            if (null != existsToken)
            {
                var result = await RemoveRefreshToken(existsToken);
            }
            _ctx.RefreshTokens.Add(rt);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken existsToken)
        {
            _ctx.RefreshTokens.Remove(existsToken);
            return await _ctx.SaveChangesAsync() > 0;
        }
        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }
            return false;
        }
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }
        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }


    }
}