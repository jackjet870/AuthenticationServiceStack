using AuthServiceStack.AuthModel.EF;

namespace AuthCenter
{
    public class AppAuthContext : AuthContext
    {
        public AppAuthContext()
            : base("EFAuthContext")//Shared.confs.authConnectionConfig)
        {

        }
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            //EF 数据库对象->实体初始化工作
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
        
        ////public List<MallAuth.Models.Client> Clients { get { return test; } }
        //public System.Data.Entity.DbSet<Client> Clients { get; set; }
        //public System.Data.Entity.DbSet<RefreshToken> RefreshTokens { get; set; }
        //public System.Data.Entity.DbSet<CustomUserClaim> CustomUserClaims { get; set; }
        //public System.Data.Entity.DbSet<ContentManagerSystem> CMS { get; set; }//Directory
        //public System.Data.Entity.DbSet<Directory> Directories { get; set; }//Directory
    }
}