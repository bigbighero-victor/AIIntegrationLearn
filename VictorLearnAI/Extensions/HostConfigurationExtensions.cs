using Victor.Framework.Infrastructure.Configurations;

namespace VictorLearnAI.Web.Extensions;

// 小 shim：放在主项目中，负责把宿主的 environment + content root 信息传给类库核心方法
public static class HostConfigurationExtensions
{
    // 最小化使用：主项目在 Program.cs 只需一行 builder.UseAppConfiguration<AppSettings>();
    public static WebApplicationBuilder UseAppConfiguration<TSettings>(this WebApplicationBuilder builder,
        bool useFlexibleNameMatching = true, string baseResourceFileName = "appsettings.base.json")
        where TSettings : AppConfigurationSettings, new()
    {
        // 1) 先把类库嵌入的 base.json 加入配置
        builder.Configuration.AddEmbeddedBaseJson(baseResourceFileName);

        // 2) 再加载主项目的 environment-specific 文件（例如 dev.json 或 production.json），覆盖 base
        //    这里自动从主项目的 ContentRoot（通常是项目根/输出目录）读取 {Environment}.json
        var envFile = $"appsettings.{builder.Environment.EnvironmentName}.json";
        builder.Configuration.AddJsonFile(envFile, optional: true, reloadOnChange: true);

        // 3) 允许主项目再加载其它自定义文件（可由主项目在 Program.cs 之前添加）
        // 4) 最后把合并后的 IConfiguration 绑定到 TSettings 并注册到 DI
        builder.Services.AddAppConfiguration<TSettings>(builder.Configuration, useFlexibleNameMatching);

        return builder;
    }
}