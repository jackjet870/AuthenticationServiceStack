using AuthServiceStack.AuthModel.EF.Models;
using AuthServiceStack.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration;

namespace AuthServiceStack.AuthModel.EF
{
    public class AuthContext : Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext<ApplicationUser>
    {

        public AuthContext()
            : base("AuthCenter_DefaultShared")
        {

        }
        public AuthContext(string Context)
            : base(Context)
        {

        }

        public static List<Client> ClientSamples
        {
            get
            {
                return new List<Client> {
               new Client
                {
                    Id = "javascriptApp",
                    Secret = Helper.GetHash("abc@123"),
                    Name = "AngularJS front-end Application",
                    ApplicationType =ApplicationTypes.JavaScript,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*",//或者www.URL,*,如果置空，由SimpleOathAthencitactionServerProvider处理
                },
               new Client
                    {
                        Id = "consoleApp",
                        Secret = Helper.GetHash("123@abc"),
                        Name = "Console Application",
                        ApplicationType = ApplicationTypes.NativeConfidential,
                        Active = true,
                        RefreshTokenLifeTime = 14400,
                        AllowedOrigin = "*"
               }};
            }
        }
        public static IEnumerable<UserModel> usersPreBuiltIn
        {
            get
            {
                return new[] {
                    new UserModel {  UserName ="Admin", Password= "admin123!@#" },
                    new UserModel {  UserName="test", Password="test-abc-123"} };
            }
        }


        //public List<MallAuth.Models.Client> Clients { get { return test; } }
        public System.Data.Entity.DbSet<Client> Clients { get; set; }
        public System.Data.Entity.DbSet<RefreshToken> RefreshTokens { get; set; }
        public System.Data.Entity.DbSet<CustomUserClaim> CustomUserClaims { get; set; }
        //受 IdentiyManager支持的用户User claim
        //public System.Data.Entity.DbSet<IdentityUserClaim> UserClaims { get; set; }
        //public System.Data.Entity.DbSet<ContentManagerSystem> CMS { get; set; }//Directory
        //public System.Data.Entity.DbSet<Directory> Directories { get; set; }//Directory

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();


            // Keep this: 用户角色管理模块
            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");
            EntityTypeConfiguration<ApplicationUser> userETC = modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");

            //AspNetUsers 特性!
            userETC.Property((ApplicationUser u) => u.UserName).IsRequired();

            // EF won't let us swap out IdentityUserRole for ApplicationUserRole here:
            modelBuilder.Entity<ApplicationUser>().HasMany<IdentityUserRole>((ApplicationUser u) => u.Roles);
            ///设定 IdentityUserRole 逻辑关系表的 主键,注意此处,lambda表达式,用到了联合主键!匿名Type!
            modelBuilder.Entity<IdentityUserRole>().HasKey((IdentityUserRole r) => new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");


            // Leave this alone:
            //IdentityUserLogin 的配置 entityTypeConfiguration,lambda表达式,用到了联合主键!匿名Type!
            EntityTypeConfiguration<IdentityUserLogin> loginETC
            = modelBuilder.Entity<IdentityUserLogin>().HasKey((IdentityUserLogin IUL) =>
                    new
                    {
                        UserId = IUL.UserId,
                        LoginProvider = IUL.LoginProvider,
                        ProviderKey = IUL.ProviderKey
                    }).ToTable("AspNetUserLogins");
            //配置一个必要的关系,实体实例不会存到数据库除非这个关系已指定.外键不能为空
            //loginETC.HasRequired<IdentityUser>((IdentityUserLogin u) => u.UserId);
            #region Create UserClaim HasRequiredKey(foreignKey) to User
            //User Claim配置 //本应用未使用该表
            EntityTypeConfiguration<IdentityUserClaim> IUC =
                modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");

            //IUC.HasRequired<IdentityUser>((IdentityUserClaim u) => u.User);
            #endregion
            // Add this, so that IdentityRole can share a table with ApplicationRole:
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");

            // Change these from IdentityRole to ApplicationRole:
            //EntityTypeConfiguration<ApplicationRole> RoleETC =
            //    modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");

            //RoleETC.Property((ApplicationRole r) => r.Name).IsRequired();
        }
    }


}