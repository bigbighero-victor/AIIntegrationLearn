using Microsoft.Extensions.Configuration;

namespace Victor.Framework.Infrastructure.Configurations;

public class AppConfigurationSettings
{
    [ConfigurationKeyName("rabbit_mq")]
    public RabbitMqSettings RabbitMqSettings { get; set; }
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