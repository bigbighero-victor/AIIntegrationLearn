using Microsoft.Extensions.Configuration;
using Victor.Framework.Infrastructure;

namespace VictorLearnAI.AISupporters;

public class AISupporterSettings : AppConfigurationSettings
{
    [ConfigurationKeyName("qwen")]
    public QwenSetting QwenSetting { get; set; }
}

public class QwenSetting
{
    [ConfigurationKeyName("base_uri")]
    public string BaseUri { get; set; }
    [ConfigurationKeyName("chat_url")]
    public string ChatUrl { get; set; }
}