<h1>Implemnts of Token-based Authentication using EntityFramework/SqlServer,wrapped as components for your Restful-App </h1>

<b>Install Directs:</b>

AuthServiceStack.AuthModel.EF.AuthContext wraps basic models for claims-based Identity Authentication <i>the super set of role-base identity</i><br/>

So you can inherit it like <br/>
<pre>
  <code>public AppAuthContext()
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
 AuthServiceStack.AuthModel.EF.AuthWithClaimsRepository wraps common-used operations for authentication && authorization<br/>

 So you can inherit it like <br/>
<pre>
  <code>public class AppAuthRepository : AuthServiceStack.AuthModel.EF.AuthWithClaimsRepository
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

<br>
<b>How to generate an Authentication Database instance?</b><br/>
Carefully setup your connectionStrings section in Web.config<br/>
Run the following command on your Package-Manage-console targetting  "AuthServiceStack.AuthModel.EF" project<br/>
Or your App (your app should inherits models/entities of "AuthServiceStack.AuthModel.EF" )<br/>
<pre>
  <code>enable-migrations 
add-migartion
update-database
 </code>
</pre>


<h1>Resource-Server Application's Consume Authentication-Filter<h1>
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
TransClaimsAuthorizationFilter is abstract and its property <i>ClaimsAbbreviationDictionary<i> is delegate object <br/>
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