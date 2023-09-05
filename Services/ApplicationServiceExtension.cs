using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSN.Data;
using MSN.Models;
using MSN.Seeding;

namespace MSN.Services
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
           IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
                opt.EnableSensitiveDataLogging();
            });

            services.AddCors();


            return services;


        }

    }
}