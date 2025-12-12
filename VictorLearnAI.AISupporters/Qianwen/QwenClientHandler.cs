namespace VictorLearnAI.AISupporters.Qianwen;

public class QwenClientHandler(AISupporterSettings  settings) : IAiSupporterService
{
    private const string API_KEY_NAME = "DASHSCOPE_QWEN_API_KEY";
    
    private string GetApiKey()
    {
        var apiKey = Environment.GetEnvironmentVariable(API_KEY_NAME);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        }
        return apiKey;
    }

    public Task<string> ChatWithAi()
    {
        var apiKey = GetApiKey();
        var combinedUri = new Uri(new Uri(settings.QwenSetting.BaseUri), settings.QwenSetting.ChatUrl);
        return Task.FromResult(combinedUri.ToString());
    }
}