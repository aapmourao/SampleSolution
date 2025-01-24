namespace SharedKernel.Infrastructure.Persistence;

public interface IProviderSqlAdapter
{
    string GetAddDaysToCurrentDateTime(string sqlCommand, int days);
}