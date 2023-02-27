using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;

namespace MyMongoInterface.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddMongoDbContext<TContext>(this IServiceCollection services, string database, IConfiguration configuration) where TContext : class
    {
        services.AddSingleton(options =>
        {
            return (Activator.CreateInstance(typeof(TContext), database, configuration) as TContext)!;
        });
    }
}
