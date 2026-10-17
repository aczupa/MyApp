
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.api.Data;
using System.Text.Json.Serialization;

namespace MyApp.api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy => policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                var admin = await userManager.FindByEmailAsync("test@test.com");
                if (admin != null && !await userManager.IsInRoleAsync(admin, "Admin"))
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngular");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapIdentityApi<IdentityUser>();
            app.MapControllers();
            app.Run();
        }
    }
}
