using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Victor.Commons;
using Victor.Framework.Infrastructure.AutoMapper;
using Victor.Framework.Infrastructure.EFCore;

namespace VictorLearnAI.Web.Extensions;

/// <summary>
/// WebApplicationBuilderExtensions, ServiceCollectionExtensions
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <param name="builder"></param>
    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        /// 将与 WebApplicationBuilder 相关的配置集中到此处：
        /// - 注册服务（调用 AddVictorLearnServices）
        /// - 绑定 AppSettings（原来的 builder.UseAppConfiguration&lt;AppSettings&gt;()）
        /// 方便 Program.cs 保持极简。
        /// </summary>
        public WebApplicationBuilder ConfigureWebApplicationBuilder()
        {
            // 绑定应用配置（保留原有行为）
            builder.UseAppConfiguration<AppSettings>();
            // 注册 services（在 ServiceCollectionExtensions 中）
            builder.Services.ServiceCollectionExtensions(builder.Configuration);
            return builder;
        }

        /// <summary>
        /// Configure AutoMapper
        /// </summary>
        /// <param name="mapperConfiguration"></param>
        /// <returns></returns>
        public WebApplicationBuilder ConfigureMapper(Action<IMapperConfigurationExpression> mapperConfiguration)
        {
            builder.Services.AddAutoMapperWithConfiguration(mapperConfiguration);
            return builder;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerActions"></param>
        /// <returns></returns>
        public WebApplicationBuilder AddServiceGroup( Action<IServiceCollection> registerActions)
        {
            builder.Services.AddServiceGroup(registerActions);
            return builder;
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// 将服务注册集中在这里，便于维护与单元测试。
        /// 在此添加 DbContext、身份认证、HttpClients 等。
        /// </summary>
        private IServiceCollection ServiceCollectionExtensions(IConfiguration configuration)
        {
            // 控制器
            services.AddControllers();

            // OpenAPI
            services.AddOpenApi();

            // 将 AppSettings 绑定到 IOptions<AppSettings>（如果需要）
            services.Configure<AppSettings>(configuration);

            // 在此注册更多服务，例如：
            var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
            services.AddAllDbContexts((sp, ctx) =>
            {
                var appSettings = sp.GetService<AppSettings>();
                var connStr = appSettings.DatabaseSettings.ConnectionString;
                ctx.UseNpgsql(connStr);
            }, assemblies);

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerActions"></param>
        /// <returns></returns>
        public IServiceCollection AddServiceGroup(Action<IServiceCollection> registerActions)
        {
            registerActions(services);
            return services;
        }
    }
}