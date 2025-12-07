using Victor.Framework.Infrastructure.Configurations;

namespace VictorLearnAI.Web;

public class AppSettings : AppConfigurationSettings
{
    [ConfigurationKeyName("database")]
    public DatabaseSetting DatabaseSetting { get; set; }
}

public class DatabaseSetting
{
    [ConfigurationKeyName("database_name")]
    public string Name { get; set; }
}