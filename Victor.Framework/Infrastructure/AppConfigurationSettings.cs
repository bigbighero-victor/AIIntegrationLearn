using Microsoft.Extensions.Configuration;

namespace Victor.Framework.Infrastructure;

public class AppConfigurationSettings
{
    [ConfigurationKeyName("rabbit_mq")]
    public RabbitMqSettings RabbitMqSettings { get; set; }
    [ConfigurationKeyName("database_config")]
    public DatabaseSettings DatabaseSettings { get; set; }
}

public class RabbitMqSettings
{
    [ConfigurationKeyName("mq_address")]
    public string MqAddress { get; set; }
    [ConfigurationKeyName("user_name")]
    public string UserName { get; set; }
    [ConfigurationKeyName("user_pwd")]
    public string UserPwd { get; set; }
}

public class DatabaseSettings
{
    [ConfigurationKeyName("host")]
    public string Host { get; set; }
    [ConfigurationKeyName("port")]
    public string Port { get; set; }
    [ConfigurationKeyName("database")]
    public string Database { get; set; }
    [ConfigurationKeyName("username")]
    public string UserName { get; set; }
    [ConfigurationKeyName("pwd")]
    public string Pwd { get; set; }
    public string ConnectionString => $"Host={Host};Port={Port};Database={Database};Username={UserName};Password={Pwd}";
}