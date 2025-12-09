using System.Diagnostics;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Victor.Framework.Infrastructure.AutoMapper;

public static class AutoMapperExtensions
{
    /// <summary>
    /// 注册 AutoMapper：不调用 BuildServiceProvider，若可用则把 ILoggerFactory 传给 MapperConfiguration 构造函数，
    /// 并通过 Mapper 的 serviceCtor (sp.GetService) 支持在 Profile 中通过构造函数注入依赖。
    /// </summary>
    public static IServiceCollection AddAutoMapperWithConfiguration(
        this IServiceCollection services,
        Action<IMapperConfigurationExpression> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // 注册 IConfigurationProvider（MapperConfiguration 的实现）
        services.AddSingleton<IConfigurationProvider>(sp =>
        {
            // 从真正的 ServiceProvider 获取 ILoggerFactory（如果有）
            var loggerFactory = sp.GetService<ILoggerFactory>();

            // 使用带或不带 ILoggerFactory 的构造函数（取决于 AutoMapper 版本）
            MapperConfiguration mapperConfig;
            if (loggerFactory != null)
            {
                // 有 ILoggerFactory 的重载时使用它（可以在配置/日志中获得更好信息）
                mapperConfig = new MapperConfiguration(configure, loggerFactory);
            }
            else
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            // 在开发/调试环境中验证配置（避免在生产环境重启失败）
            var env = sp.GetService<IHostEnvironment>();
            if ((env != null && env.IsDevelopment()) || env == null && Debugger.IsAttached)
            {
                mapperConfig.AssertConfigurationIsValid();
            }

            return mapperConfig;
        });

        // 注册 IMapper：尽量使用能够解析依赖的构造方式
        services.AddSingleton<IMapper>(sp =>
        {
            var config = sp.GetRequiredService<IConfigurationProvider>() as MapperConfiguration
                         ?? throw new InvalidOperationException("MapperConfiguration 未能正确解析。");

            // 大多数 AutoMapper 版本中，Mapper 有一个接受 serviceCtor 的构造函数：
            // new Mapper(config, sp.GetService)
            // 这允许在 Profile 的构造函数中注入 DI 服务。
            // 如果你的 AutoMapper 版本不包含这个构造函数（极少数老/新版本），
            // 请改为使用 config.CreateMapper() 或 config.CreateMapper(serviceProvider)（如果可用）。
            try
            {
                return new Mapper(config, sp.GetService);
            }
            catch (MissingMethodException)
            {
                // 兜底：如果没有接受 serviceCtor 的构造函数，尝试无参 CreateMapper
                // 注意：这将无法在 Profile 构造函数中通过 DI 注入服务。
                return config.CreateMapper();
            }
        });

        return services;
    }
}