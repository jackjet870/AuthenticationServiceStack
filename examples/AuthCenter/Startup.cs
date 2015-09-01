using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;
[assembly: Microsoft.Owin.OwinStartup(typeof(AuthCenter.Startup))]
namespace AuthCenter
{
    public class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            ConfigureAuth(app);
            var config = new System.Web.Http.HttpConfiguration();
            WebApiConfig(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }



        private void ConfigureAuth(IAppBuilder app)
        {
            var repo = new AppAuthRepository();
            Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions OAuthServerOptions =
                new Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions()
                {
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                    AllowInsecureHttp = true,
                    TokenEndpointPath = new Microsoft.Owin.PathString("/token"),
                    Provider = new AuthServiceStack.AuthProvider.EF.SimpleOAuthClaimsAuthorizationServerProvider(repo),
                    RefreshTokenProvider = new AuthServiceStack.AuthProvider.EF.SimpleFreshTokenProvider(repo)

                };
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());

        }
        private void WebApiConfig(HttpConfiguration config)
        {
            // Web API routes// Attribute routing.
            config.MapHttpAttributeRoutes();

            // Convention-based routing.ignore when 'Attribute routing' explicitly specific
            config.Routes.MapHttpRoute(
                name: "ResourceApi",
                routeTemplate: "api/rsc/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
              name: "ActionApi",
              routeTemplate: "api/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }
            );

            //config.Filters.Add(new Thinktecture.IdentityModel.Authorization.WebApi.ClaimsAuthorizeAttribute());

            var jsonFormatter = config.Formatters.OfType<System.Net.Http.Formatting.JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

        }
    }
}
