using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSN.Data;
using MSN.Models;
using MSN.Seeding;
using MSN.Services;
using MSN.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Extensions :
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = 102400000;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200"));


app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<IdentityRole>>();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    context.Database.Migrate();  // This line ensures that pending migrations are applied.
    try
    {
        DataSeeder.SeedDataAsync(context, userManager, roleManager).Wait();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding data: {ex.Message}");
    }
}

app.MapControllers();


app.MapHub<MessageHub>("/hubs/message");

//using (var scope = app.Services.CreateScope())
//{
//    var serviceProvider = scope.ServiceProvider;
//    var context = serviceProvider.GetRequiredService<DataContext>();
//    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


//    context.Database.Migrate();
//    try
//    {
//       await DataSeeder.SeedData(context, userManager, roleManager);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"An error occurred while seeding data: {ex.Message}");
//    }

//}

app.Run();
