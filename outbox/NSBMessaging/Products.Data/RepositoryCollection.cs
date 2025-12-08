using Microsoft.Extensions.DependencyInjection;

namespace Products.Data
{
    public static class RepositoryCollection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {

            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }
    }
}
