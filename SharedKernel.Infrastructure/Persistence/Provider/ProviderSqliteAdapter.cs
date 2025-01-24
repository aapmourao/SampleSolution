namespace SharedKernel.Infrastructure.Persistence;

public class ProviderSqliteAdapter: IProviderSqlAdapter
{
    public string GetAddDaysToCurrentDateTime(string sqlCommand, int days)
    {
        return sqlCommand.Replace("#sqlToken#", $"datetime('now', '-{days} days')");
    }
}
