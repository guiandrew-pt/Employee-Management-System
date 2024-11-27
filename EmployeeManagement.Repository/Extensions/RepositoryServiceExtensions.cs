using EmployeeManagement.Repository.Data;
using EmployeeManagement.Repository.Repositories;
using EmployeeManagement.Repository.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Repository.Extensions
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services, string connectionString)
        {
            // Register MySqlConnectionFactory with the provided connection string
            services.AddSingleton<MySqlConnectionFactory>(_ => new MySqlConnectionFactory(connectionString));

            // Register the repository
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}

