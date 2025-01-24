using System.Text.Json.Serialization;
using SharedKernel.Infrastructure.Persistence;

namespace SharedKernel.Infrastructure.Common.Settings;
public class DatabaseSelectorOptions
{
    public static string Section = "DatabaseSelector";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProviderType Provider { get; set; } = ProviderType.Sqlite;
}
