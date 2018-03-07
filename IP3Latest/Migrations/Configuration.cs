namespace IP3Latest.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IP3Latest.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(IP3Latest.Models.ApplicationDbContext context)
        {
            // Seed initial data only if the database is empty
            if (!context.Users.Any())
            {
                var adminEmail = "admin@admin.com";
                var adminUserName = "Admin1";
                var adminFirstName = "System Administrator";
                var adminLastName = "System Administrator";
                var adminPassword = "Admin_1";
                bool adminArchived = false;
                string adminRole = "Administrator";

                var docauthEmail = "docauth@docauth.com";
                var docauthUserName = "DocAuth1";
                var docauthFirstName = "Document Author 1";
                var docauthLastName = "Document Author 1";
                var docauthPassword = "DocAuth_1";
                bool docauthArchived = false;
                string docauthRole = "Document Author";

                var distributeeEmail = "distributee@distributee.com";
                var distributeeUserName = "Distributee1";
                var distributeeFirstName = "Distributee 1";
                var distributeeLastName = "Distributee 1";
                var distributeePassword = "Distributee_1";
                bool distributeeArchived = false;
                string distributeeRole = "Distributee";

                CreateAdminUser(context, adminEmail, adminUserName, adminFirstName, adminLastName, adminPassword, adminArchived, adminRole);
                CreateDocAuthUser(context, docauthEmail, docauthUserName, docauthFirstName, docauthLastName, docauthPassword, docauthArchived, docauthRole);
                CreateDistributeeUser(context, distributeeEmail, distributeeUserName, distributeeFirstName, distributeeLastName, distributeePassword, distributeeArchived, distributeeRole);
            }
        }

        private void CreateAdminUser(IP3Latest.Models.ApplicationDbContext context, string adminEmail, string adminUserName, string adminFirstName, string adminLastName, string adminPassword, bool adminArchived, string adminRole)
        {
            // Create the "admin" user
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,

                Email = adminEmail,

                FirstName = adminFirstName,

                LastName = adminLastName,

                Archived = adminArchived,

            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var userCreateResult = userManager.Create(adminUser, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            // Create the "Administrator" role
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole(adminRole));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            // Add the "admin" user to "Administrator" role
            var addAdminRoleResult = userManager.AddToRole(adminUser.Id, adminRole);
            if (!addAdminRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addAdminRoleResult.Errors));
            }
        }

        private void CreateDocAuthUser(IP3Latest.Models.ApplicationDbContext context, string docauthEmail, string docauthUserName, string docauthFirstName, string docauthLastName, string docauthPassword, bool docauthArchived, string docauthRole)
        {
            // Create the "admin" user
            var docauthUser = new ApplicationUser
            {
                UserName = docauthUserName,

                Email = docauthEmail,

                FirstName = docauthFirstName,

                LastName = docauthLastName,

                Archived = docauthArchived,
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var userCreateResult = userManager.Create(docauthUser, docauthPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            // Create the "Administrator" role
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole(docauthRole));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            // Add the "admin" user to "Administrator" role
            var addDocAuthRoleResult = userManager.AddToRole(docauthUser.Id, docauthRole);
            if (!addDocAuthRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addDocAuthRoleResult.Errors));
            }
        }

        private void CreateDistributeeUser(IP3Latest.Models.ApplicationDbContext context, string distributeeEmail, string distributeeUserName, string distributeeFirstName, string distributeeLastName, string distributeePassword, bool distributeeArchived, string distributeeRole)
        {
            // Create the "admin" user
            var distributeeUser = new ApplicationUser
            {
                UserName = distributeeUserName,

                Email = distributeeEmail,

                FirstName = distributeeFirstName,

                LastName = distributeeLastName,

                Archived = distributeeArchived,
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var userCreateResult = userManager.Create(distributeeUser, distributeePassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            // Create the "Administrator" role
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole(distributeeRole));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            // Add the "admin" user to "Administrator" role
            var addDistributeeRoleResult = userManager.AddToRole(distributeeUser.Id, distributeeRole);
            if (!addDistributeeRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addDistributeeRoleResult.Errors));
            }
        }
    }
}
