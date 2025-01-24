namespace SharedKernel.Infrastructure.Persistence;

/// <summary>
/// Hold information regarding the scripts that run to create/update the database.
/// Scripts should be under a folder with version number and follow the following template:
/// Folder name (vX.XX.XX) --> Version field following semantic version
///  ex: v1.01.02 --> Version field
/// File name (XXX_<Description>.sql) --> Script field
///  ex: 001_CreateTable.sql
/// </summary>
public class DbVersion
{
    public string Version { get; set; } = null!;
    public string Script { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; } = null;
    public int ExecutionTime { get; set; } = 0;
    public byte Success { get; set; } = 0;
}