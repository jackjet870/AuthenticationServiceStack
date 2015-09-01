namespace AuthServiceStack.AuthModel.EF.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using core;

    internal sealed class Configuration : DbMigrationsConfiguration<AuthServiceStack.AuthModel.EF.AuthContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AuthServiceStack.AuthModel.EF.AuthContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            if (context.Clients.Count() == 0)
            {
                context.Clients.AddRange(AuthContext.ClientSamples);
                context.SaveChanges();
            }
            if (context.Users.Count() == 0)
            {
                var m = new AuthRepository(context);
                foreach (var user in AuthContext.usersPreBuiltIn)
                {
                    m.RegisterUser(user.UserName, user.Password);
                }

            }


        }
    }
}
