using Microsoft.EntityFrameworkCore;
using Victor.Commons;
using Victor.Framework.Infrastructure.EFCore;

namespace VictorLearnAI.Web.Extensions;

/// <summary>
/// WebApplicationBuilderExtensions, ServiceCollectionExtensions
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// 将与 WebApplicationBuilder 相关的配置集中到此处：
    /// - 注册服务（调用 AddVictorLearnServices）
    /// - 绑定 AppSettings（原来的 builder.UseAppConfiguration&lt;AppSettings&gt;()）
    /// 方便 Program.cs 保持极简。
    /// </summary>
    public static WebApplicationBuilder ConfigureWebApplicationBuilder(this WebApplicationBuilder builder)
    {
        // 绑定应用配置（保留原有行为）
        builder.UseAppConfiguration<AppSettings>();
        // 注册 services（在 ServiceCollectionExtensions 中）
        builder.Services.ServiceCollectionExtensions(builder.Configuration, builder.Environment);
        return builder;
    }

    /// <summary>
    /// 将服务注册集中在这里，便于维护与单元测试。
    /// 在此添加 DbContext、身份认证、HttpClients 等。
    /// </summary>
    public static IServiceCollection ServiceCollectionExtensions(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment _)
    {
        // 控制器
        services.AddControllers();

        // OpenAPI
        services.AddOpenApi();

        // 将 AppSettings 绑定到 IOptions<AppSettings>（如果需要）
        //services.Configure<AppSettings>(configuration);

        // 在此注册更多服务，例如：
        // services.AddDbContext<MyDbContext>(...);
        // services.AddScoped<IMyService, MyService>();
        // services.AddAuthentication(...);
        var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
        services.AddAllDbContexts(ctx =>
        {
            var appSettings = services.BuildServiceProvider().GetService<AppSettings>();
            var connStr = configuration.GetValue<string>(appSettings.DatabaseSettings.ConnectionString);
            ctx.UseNpgsql(connStr);
        }, assemblies);
        return services;
    }
}