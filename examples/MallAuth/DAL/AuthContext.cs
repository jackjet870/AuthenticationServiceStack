using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MallAuth.DAL
{
    
    public class AppAuthContext : AuthServiceStack.AuthModel.EF.AuthContext
    {
        public AppAuthContext()
            : base("LocalAuth")//Shared.confs.authConnectionConfig)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //EF 数据库对象->实体初始化工作
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
        //public static List<Client> test = new List<Client> {        
        //       new Client
        //        {
        //            Id = "MallConsole",
        //            Secret = Helper.GetHash("abc@123"),
        //            Name = "AngularJS front-end Application",
        //            ApplicationType =ApplicationTypes.JavaScript,
        //            Active = true,
        //            RefreshTokenLifeTime = 7200,
        //            AllowedOrigin = "*",//或者www.URL,*,如果置空，由SimpleOathAthencitactionServerProvider处理
        //        },
        //     new Client
        //        {
        //            Id = "consoleApp",
        //            Secret = Helper.GetHash("123@abc"),
        //            Name = "Console Application",
        //            ApplicationType = ApplicationTypes.NativeConfidential,
        //            Active = true,
        //            RefreshTokenLifeTime = 14400,
        //            AllowedOrigin = "*"
        //        }
        //};
        ////public List<MallAuth.Models.Client> Clients { get { return test; } }
        //public System.Data.Entity.DbSet<Client> Clients { get; set; }
        //public System.Data.Entity.DbSet<RefreshToken> RefreshTokens { get; set; }
        //public System.Data.Entity.DbSet<CustomUserClaim> CustomUserClaims { get; set; }
        //public System.Data.Entity.DbSet<ContentManagerSystem> CMS { get; set; }//Directory
        //public System.Data.Entity.DbSet<Directory> Directories { get; set; }//Directory
    }

}