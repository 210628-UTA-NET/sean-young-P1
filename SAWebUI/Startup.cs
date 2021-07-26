using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAWebUI {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAuthentication()
                .AddGoogle(options => {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });
            

            services.AddDefaultIdentity<CustomerUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddRoles<IdentityRole>()
             .AddEntityFrameworkStores<SADL.SADBContext>();

            services.AddControllersWithViews(); //.AddRazorRuntimeCompilation();
            services.AddDbContext<SADL.SADBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AzureDB")));
            services.AddScoped(typeof(SADL.ICRUD<>), typeof(SADL.StoreModelDB<>));
            services.AddScoped<SABL.CustomerManager>();
            services.AddScoped<SABL.StateManager>();
            services.AddScoped<SABL.StorefrontManager>();
            services.AddScoped<SABL.LineItemManager>();
            services.AddScoped<SABL.ShoppingCartManager>();
            services.AddScoped<SABL.OrderManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
