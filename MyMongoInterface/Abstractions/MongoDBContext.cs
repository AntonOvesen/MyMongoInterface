using MongoDB.Driver;

namespace MyMongoInterface.Abstractions
{
    public abstract class MongoDBContext
    {
        protected readonly IMongoDatabase database;
        protected readonly MongoClient client;

        public MongoDBContext(string database, IConfiguration configuration)
        {
            this.client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            this.database = client.GetDatabase(database);
        }

        public async Task ExecuteTransactionAsync<TResult>(Func<IClientSessionHandle, CancellationToken, Task<TResult>> transaction)
        {
            using (var session = client.StartSession())
            {
                await session.WithTransactionAsync(transaction);
            }
        }

        public void ExecuteTransaction<TResult>(Func<IClientSessionHandle, CancellationToken, TResult> transaction)
        {
            using (var session = client.StartSession())
            {
                session.WithTransaction(transaction);
            }
        }
    }
}
