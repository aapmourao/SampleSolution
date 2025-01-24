namespace SharedKernel.Infrastructure.Persistence.Provider
{
    public class ProviderSqlAdapterFactory
    {
        public static IProviderSqlAdapter CreateProviderSqlAdapter(ProviderType provider)
        {
            return provider switch
            {
                ProviderType.Sqlite => new ProviderSqliteAdapter(),
                ProviderType.Postgres => new ProviderPostgreSqlAdapter(),
                _ => throw new NotImplementedException($"The provider sql adapter for {provider} is not implemented.")
            };
        }
    }
}