using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System;

[assembly: OwinStartupAttribute(typeof(kl_eCom.Web.Startup))]
namespace kl_eCom.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRoles();
            checkForAdminAsync();
        }

        private void createRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists("Customer"))
            {
                var role = new IdentityRole
                {
                    Name = "Customer"
                };
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Vendor"))
            {
                var role = new IdentityRole
                {
                    Name = "Vendor"
                };
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.Create(role);
            }
        }

        private async System.Threading.Tasks.Task checkForAdminAsync()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            ApplicationDbContext db = context;

            var admin = db.EcomUsers
                          .Include(m => m.User)
                          .FirstOrDefault(m => m.User.UserName == "klAdmin");

            if (admin is null)
            {
                var user = new ApplicationUser {
                    UserName = "klAdmin",
                    Email = "khushlife@gmail.com",
                    DateCreated = DateTime.Now,
                    DOB = new DateTime(2000, 01, 01)
                };
                user.FirstName = "FirstName";
                user.LastName = "LastName";

                IdentityResult result;
                try
                {
                    result = await userManager.CreateAsync(user, "KLadmin@123");

                    if (result.Succeeded)
                    {
                        db.EcomUsers.Add(new EcomUser {
                            ApplicationUserId = user.Id,
                            PrimaryRole = "Admin" 
                        });
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            userManager.Delete(user);
                        }

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        await userManager.AddToRolesAsync(user.Id, "Admin");

                        return;
                    }
                    else
                    {
                        throw new System.Exception();
                    }
                }
                catch (System.Exception ex)
                {

                }
            }
            else if(admin.PrimaryRole != "Admin")
            {
                admin.PrimaryRole = "Admin";
                context.Entry(admin).State = System.Data.Entity.EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
