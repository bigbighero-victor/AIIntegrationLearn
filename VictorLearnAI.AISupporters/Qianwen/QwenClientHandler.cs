namespace VictorLearnAI.AISupporters.Qianwen;

public class QwenClientHandler
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

    public QwenClientHandler(AISupporterSettings  settings)
    {
        
    }
}