using Microsoft.AspNetCore.Authentication.Cookies;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories;

namespace OutOfOffice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.LoginPath = "/account/SignIn";
               });
            builder.Services.AddScoped(provider =>
            {
                var connectionString = builder.Configuration["ConnectionString"];
                return new OutOfOfficeDbContext(connectionString);
            });
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ModelProfile>();
            });
            builder.Services.AddScoped<EmployeesRepository>();
            builder.Services.AddScoped<LeaveRequestsRepository>();
            builder.Services.AddScoped<ApprovalRequestsRepository>();
            builder.Services.AddSingleton<LeaveRequestDateValidator>();
            builder.Services.AddSingleton<ProjectsRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
