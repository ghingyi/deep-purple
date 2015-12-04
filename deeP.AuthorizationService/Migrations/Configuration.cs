namespace deeP.AuthorizationService.Migrations
{
    using deeP.AuthorizationService.Infrastructure;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<deeP.AuthorizationService.Infrastructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(deeP.AuthorizationService.Infrastructure.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            if (roleManager.Roles.Count() == 0)
            {
                //roleManager.Create(new IdentityRole { Name = InternalConstants.RoleNames.Admin });
                // The following two roles are enough for this very simple sample
                roleManager.Create(new IdentityRole { Name = InternalConstants.RoleNames.Seller });
                roleManager.Create(new IdentityRole { Name = InternalConstants.RoleNames.Buyer });
            }
        }
    }
}
