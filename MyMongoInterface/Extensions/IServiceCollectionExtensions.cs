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

    public static void AddPlugins(this IServiceCollection services)
    {
        var configurations = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(o => o.GetTypes())
            .Where(o => o.IsClass && !o.IsAbstract && o.IsSubclassOf(typeof(IPluginBase)))
            .Select(o => (IPluginBase)Activator.CreateInstance(o)!)
            .Where(o => o is not null)
            .ToList();
        


    }
}
