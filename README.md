# Implements of Token-based Authentication using EntityFramework/SqlServer,
to be wrapped as components for your Restful-App  

Thanks to Taiseer Joudeh and his great articals: [Token Based Authentication using ASP.NET Web API 2, Owin, and Identity](http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity)
Also his open-source [projects](https://github.com/tjoudeh/AngularJSAuthenticationion")
on github 

## Your apps consume _AuthServiceStack_ as
```csharp
  [RoutePrefix("api/TestResource")]
    public class TestResourceController : ApiController
    {
    	//MyClaimAuthorizationFilter from "AuthServiceStack.AuthProvider.TransClaimsAuthorizationFilter" you can rewrite it
        [MyClaimAuthorizationFilter] // the user must have claims 'get' "TestResource"
        public IHttpActionResult Get()
        {
            return Ok(Order.CreateOrders());
        }
		[MyClaimAuthorizationFilter("Role","Admin")] // the user must have claims 'role' "Admin"
        public IHttpActionResult Other()
        {
            return Ok(Order.CreateOrders());
        }
    }
```

##Configuration of your Auths-App
```csharp
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
		...
```	 

##Here are 5 main classes to be derived in your app,the following as example:
###1.AuthServiceStack.AuthModel.EF.AuthContext which wraps basic models for claims-based identity.
As you know,Claims-based identity is _a super set of role-base identity_  

So you can inherit it like
```csharp
public class AppAuthContext:AuthContext {
  public AppAuthContext()
            : base("EFAuthContext")
        {

        }
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            //Database object initiation..
            base.OnModelCreating(modelBuilder);
			//other thing..
            //modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
```
### 2.AuthServiceStack.AuthModel.EF.AuthWithClaimsRepository 
which wraps common-used operations for authentication && authorization,such as registering/remove user.. 

 So you can inherit it like 
```csharp
  public class AppAuthRepository : AuthServiceStack.AuthModel.EF.AuthWithClaimsRepository
    {
        public AppAuthRepository() :
            base(new AppAuthContext(), ClaimsAbbreviationDictionary.dict)
        {}
        public override List<System.Security.Claims.Claim> getUserClaims(string username)
        {
            //todo 
            //your codes to design your user-claims, such as" this._ctx.CustomUserClaims.."
            return base.getUserClaims(username);//using table <i>CustomUserClaim</i> of Auth-Database,you can override it!
        }
    }
```
### 3.AuthServiceStack.AuthProvider.SimpleOAuthClaimsAuthorizationServerProvider
which wraps clients authentication,token-generating/ticket with identities

### 4.AuthServiceStack.AuthProvider.SimpleFreshTokenProvider
which wraps token-refresh.

### 5.Resource-Server Application's Consume Authentication-Filter 
#### How to generate an Authentication Database instance? 
Setup your connectionStrings section in Web.config,you can refer to the example _AuthCenter_
Run the following command on your Package-Manage-console targetting  "AuthServiceStack.AuthModel.EF" project 
Or your App (your app should inherits models/entities of "AuthServiceStack.AuthModel.EF" ) 
```
enable-migrations 
add-migartion
update-database
```
```csharp
class MyClaimAuthorizationFilterAttribute : AuthServiceStack.AuthProvider.TransClaimsAuthorizationFilter
    {
        public MyClaimAuthorizationFilterAttribute(string claimType = null,string claimValue = null)
        {
            this.ClaimType = claimType; //null as to get action-verb
            this.ClaimValue = claimValue; //null as to get controller's name
            this.ClaimsAbbreviationDictionary = () => your ClaimsAbbreviationDictionary ...;//delegate function to return Dictionary<string,string>

        }
		public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
```
## Important! 
The AuthServiceStack.AuthProvider.TransClaimsAuthorizationFilter is abstract and its property _ClaimsAbbreviationDictionary_ is *delegate* object  
you can offer a ClaimsAbbreviationDictionary to translate abbr claim to long-unique claim-type  
Or remain defaults ,i.e a null object to keep raw claim-type.



