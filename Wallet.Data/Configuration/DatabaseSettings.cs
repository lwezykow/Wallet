namespace Wallet.Data.Configuration;

public class DatabaseSettings
{
    public const string SectionName = "Database";
    
    public required string ConnectionString { get; set; }
}