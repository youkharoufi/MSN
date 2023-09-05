using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSN.Data;
using MSN.Models;
using MSN.Seeding;
using MSN.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Extensions :
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<DataContext>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    context.Database.Migrate();
    try
    {
        DataSeeder.SeedData(context, userManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding data: {ex.Message}");
    }

}

app.Run();
