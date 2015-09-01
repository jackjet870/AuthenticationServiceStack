<h6>Implements of Token-based Authentication using EntityFramework/SqlServer,
to be wrapped as components for your Restful-App </h6>

Thanks to Taiseer Joudeh and his great articals: <a href="http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/">Token Based Authentication using ASP.NET Web API 2, Owin, and Identity</a>
<br/>Also his open-source projects on github  <a href="https://github.com/tjoudeh/AngularJSAuthenticationion">tjoudeh/AngularJSAuthenticationion</a>
<h1>4 Main Classes:</h1>

<h2>1.AuthServiceStack.AuthModel.EF.AuthContext</h2> wraps basic models for claims-based identity.<br/>
<b>claims-based identity is<i>a super set of role-base identity</i></b><br/>

So you can inherit it like <br/>
<pre>
  <code>
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
 </code>
</pre>
 <h2>2.AuthServiceStack.AuthModel.EF.AuthWithClaimsRepository</h2> wraps common-used operations for authentication && authorization,such as register user..<br/>

 So you can inherit it like <br/>
<pre>
  <code>
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
 </code>
</pre>
 <br>
 <h2>
   3.AuthServiceStack.AuthProvider.SimpleOAuthClaimsAuthorizationServerProvider
 <h2> wraps clients authentication,token-generating/ticket with identities
  <h2>
   4.AuthServiceStack.AuthProvider.SimpleFreshTokenProvider
 <h2> wraps token-refresh.
<b>How to generate an Authentication Database instance?</b><br/>
Carefully setup your connectionStrings section in Web.config,you can refer to the example <i>AuthCenter</i><br/>
Run the following command on your Package-Manage-console targetting  "AuthServiceStack.AuthModel.EF" project<br/>
Or your App (your app should inherits models/entities of "AuthServiceStack.AuthModel.EF" )<br/>
<pre><code>
enable-migrations 
add-migartion
update-database
 </code>
</pre>


<h1>Resource-Server Application's Consume Authentication-Filter</h1>
<pre>
  <code>class MyClaimAuthorizationFilterAttribute : AuthServiceStack.AuthProvider.TransClaimsAuthorizationFilter
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
  </code>
</pre>
Important! <br>
TransClaimsAuthorizationFilter is abstract and its property <i>ClaimsAbbreviationDictionary</i> is delegate object <br/>
you can offer a ClaimsAbbreviationDictionary to translate abbr claim to long-unique claim-type <br/>
Or offer a null object to keep raw claim-type

<pre>
  <code>[RoutePrefix("api/TestResource")]
    public class TestResourceController : ApiController
    {
        [MyClaimAuthorizationFilter] // the user must have claims <i>get</i> "TestResource"
        public IHttpActionResult Get()
        {
            return Ok(Order.CreateOrders());
        }
		[MyClaimAuthorizationFilter("Role","Admin")] // the user must have claims <i>role</i> "Admin"
        public IHttpActionResult Other()
        {
            return Ok(Order.CreateOrders());
        }
    }
  </code>
</pre>


<h1>Configuration of your Auths-App</h1>
<pre><code>
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
</code></pre>		 