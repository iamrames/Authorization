using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Data;
using App.Models;
using System.Security.Claims;
using App.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Over15Only", policy =>
            //    policy.Requirements.Add(new MinimumAgeRequirement(15)));

            //    options.AddPolicy("TrustedUser", policy => policy.RequireAssertion(context => context.User.HasClaim(c => c.Issuer == "https://my.trustedissuer.com/")));

            //    options.AddPolicy("VisitorsOnly", policy => policy.RequireClaim("BadgeNumber", "visitor-badge"));
            //    //options.AddPolicy("TeachersOnly", policy => policy.RequireRole("Teacher"));
            //});

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditPolicy", policy => policy.Requirements.Add(new SameAuthorRequirement()));
            });

            services.AddSingleton<IAuthorizationPolicyProvider, MinimumAgePolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Admin}/{id?}");
            });

            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin", "Teacher", "Student" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var _admin = await UserManager.FindByEmailAsync("admin@admin.com");
            if (_admin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com"
                };

                var createAdmin = await UserManager.CreateAsync(admin, "Admin2019!");
                if (createAdmin.Succeeded)
                    await UserManager.AddToRoleAsync(admin, "Admin");
            }

            var _teacher = await UserManager.FindByEmailAsync("teacher@teacher.com");
            if (_teacher == null)
            {
                var teacher = new ApplicationUser
                {
                    UserName = "teacher@teacher.com",
                    Email = "teacher@teacher.com"
                };

                var createTeacher = await UserManager.CreateAsync(teacher, "Teacher2019!");
                if (createTeacher.Succeeded)
                    await UserManager.AddToRoleAsync(teacher, "Teacher");
            }

            var _student = await UserManager.FindByEmailAsync("student@student.com");
            if (_student == null)
            {
                var student = new ApplicationUser
                {
                    UserName = "student@student.com",
                    Email = "student@student.com"
                };

                var createStudent = await UserManager.CreateAsync(student, "Student2019!");
                if (createStudent.Succeeded)
                    await UserManager.AddToRoleAsync(student, "Student");
            }

            var _visitor = await UserManager.FindByEmailAsync("visitor@visitor.com");
            if (_visitor == null)
            {
                var visitor = new ApplicationUser
                {
                    UserName = "visitor@visitor.com",
                    Email = "visitor@visitor.com"
                };

                var createVisitor = await UserManager.CreateAsync(visitor, "Visitor2019!");
                if (createVisitor.Succeeded)
                    await UserManager.AddClaimAsync(visitor, new Claim("BadgeNumber", "visitor-badge"));
            }

            var _gamer = await UserManager.FindByEmailAsync("gamer@gamer.com");
            if (_gamer == null)
            {
                var gamer = new ApplicationUser
                {
                    UserName = "gamer@gamer.com",
                    Email = "gamer@gamer.com"
                };

                var createVisitor = await UserManager.CreateAsync(gamer, "Gamer2019!");
                if (createVisitor.Succeeded)
                    await UserManager.AddClaimAsync(gamer, new Claim(ClaimTypes.DateOfBirth, DateTime.Now.AddYears(-20).ToString()));
            }
        }
    }
}
