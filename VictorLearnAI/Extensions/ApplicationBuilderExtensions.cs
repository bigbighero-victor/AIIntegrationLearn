using Scalar.AspNetCore;

namespace VictorLearnAI.Web.Extensions;

/// <summary>
/// ApplicationBuilderExtensions
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 将 app.Use 与 endpoint 映射集中到此方法，保持 Program.cs 简洁。
    /// 根据环境进行差异化处理（如 Development 专用映射）。
    /// </summary>
    public static WebApplication WebApplicationExtensions(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        // 如需启用 HTTPS 重定向，取消下面注释
        // app.UseHttpsRedirection();

        // 授权中间件（如果有认证中间件，确保顺序正确）
        app.UseAuthorization();

        return app;
    }
}