using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using System.Configuration;

[assembly: OwinStartup(typeof(MallAuth.Startup))]
namespace MallAuth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            //throw new NotImplementedException();
            var repo = new DAL.AppAuthRepository();
            Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions OAuthServerOptions =
                new Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions()
                {
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                    AllowInsecureHttp = true,
                    TokenEndpointPath = new PathString("/token"),
                    Provider = new AuthServiceStack.AuthProvider.EF.SimpleOAuthClaimsAuthorizationServerProvider(repo),
                    RefreshTokenProvider = new AuthServiceStack.AuthProvider.EF.SimpleFreshTokenProvider(repo)

                };
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());
        }

    }




}