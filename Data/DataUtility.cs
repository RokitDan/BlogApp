using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BlogApp.Data

{
    public static class DataUtility
    {
        private const string _adminEmail = "dleeschelltest@gmail.com";
        private const string _adminRole = "Administrator";

        private const string _modEmail = "moderator@Mailinator.com";
        private const string _modRole = "Moderator";

        public static DateTime GetPostGresDate(DateTime datetime)
        {
            return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        }

        //public static void GetPostGresDate(this DateTime datetime)
        //{
        //    datetime = DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        //}

        //public static void GetPostGresDateTimeNow(this DateTime? datetime)
        //{
        //    datetime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        //}


        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(":");
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }


        public static async Task SeedDataAsync(IServiceProvider svcProvider)
        {
            //Service: and instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            //Migration: This is the programatic equivalent to Update Database
            await dbContextSvc.Database.MigrateAsync();

            //Service: an instance of Configuration Service
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();

            //Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //Service: An instance of UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

            //Seed Roles
            await SeedRolesAsync(roleManagerSvc);

            //Seed users
            await SeedUsersAsync(dbContextSvc, configurationSvc, userManagerSvc);



        }


        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }

            if (!await roleManager.RoleExistsAsync(_modRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_modRole));
            }
        }


        //Add Admin
        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Danny",
                    LastName = "Schellenberger",
                    PhoneNumber = "123-123-1234",
                    EmailConfirmed = true
                };
                var pwd = configuration["BlogPasswords:AdminPwd"];
                await userManager.CreateAsync(adminUser, pwd ?? Environment.GetEnvironmentVariable("AdminPwd"));
                await userManager.AddToRoleAsync(adminUser, _adminRole);

            }
        }

        //Add moderator
        private static async Task SeedModsAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            if (!context.Users.Any(u => u.Email == _modEmail))
            {
                BlogUser modUser = new()
                {
                    Email = _modEmail,
                    UserName = _modEmail,
                    FirstName = "Blog",
                    LastName = "Moderator",
                    PhoneNumber = "123-123-1234",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(modUser, configuration["ModeratorPwd"] ?? Environment.GetEnvironmentVariable("ModeratorPwd"));
                await userManager.AddToRoleAsync(modUser, _modRole);

            }

        }

    }
}
