namespace SharedKernel.Infrastructure.Persistence;

public class ProviderPostgreSqlAdapter: IProviderSqlAdapter
{
    public string GetAddDaysToCurrentDateTime(string sqlCommand, int days)
    {
        return sqlCommand.Replace("#sqlToken#", $"NOW() - INTERVAL '{days} days'");
    }
}
