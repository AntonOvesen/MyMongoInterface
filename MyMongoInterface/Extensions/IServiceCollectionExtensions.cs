namespace MyMongoInterface.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddMongoDbContext<TContext>(this IServiceCollection services, string database, IConfiguration configuration) where TContext : class
    {
        services.AddSingleton<TContext>(options =>
        {
            return (Activator.CreateInstance(typeof(TContext), database, configuration) as TContext)!;
        });
    }
}
