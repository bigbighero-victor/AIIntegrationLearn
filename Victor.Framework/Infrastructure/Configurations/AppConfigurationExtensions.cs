using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Victor.Framework.Infrastructure.Configurations
{
    public static class AppConfigurationExtensions
    {
        // 将类库内嵌的 base.json 加入 IConfigurationBuilder
        public static IConfigurationBuilder AddEmbeddedBaseJson(this IConfigurationBuilder builder,
            string resourceFileName = "base.json")
        {
            var assembly = typeof(AppConfigurationSettings).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(resourceFileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName != null)
            {
                using var s = assembly.GetManifestResourceStream(resourceName);
                if (s != null)
                {
                    builder.AddJsonStream(s);
                }
            }

            return builder;
        }

        // 把合并后的 IConfiguration 绑定到 TSettings（派生自 AppConfigurationSettings）并注册到 DI
        // useAttributeBinder: true 时会读取 ConfigurationKeyName / JsonPropertyName 特性以进行映射（推荐 true）
        public static IServiceCollection AddAppConfiguration<TSettings>(this IServiceCollection services,
            IConfiguration configuration, bool useAttributeBinder = true)
            where TSettings : AppConfigurationSettings, new()
        {
            // 保留 IOptions<T> 支持
            services.Configure<TSettings>(configuration);

            // 绑定成具体实例并注册单例
            TSettings settings;
            if (useAttributeBinder)
            {
                settings = configuration.BindUsingAttributes<TSettings>();
            }
            else
            {
                settings = configuration.Get<TSettings>() ?? new TSettings();
            }

            services.AddSingleton(settings);
            services.AddSingleton<AppConfigurationSettings>(settings);
            return services;
        }

        public static IServiceCollection AddAllAppConfigurations(this IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            IConfiguration configuration, bool useAttributeBinder = true)
        {
            var addMethod = typeof(AppConfigurationExtensions).GetMethod(
                nameof(AddAppConfiguration), BindingFlags.Public | BindingFlags.Static);
            
            foreach (var asmToLoad in assemblies)
            {
                Type[] typesInAsm = asmToLoad.GetTypes();
                foreach (var settingsType in typesInAsm
                             .Where(t => !t.IsAbstract && typeof(AppConfigurationSettings).IsAssignableFrom(t)))
                {
                    var generic = addMethod.MakeGenericMethod(settingsType);
                    generic.Invoke(null, [services, configuration, useAttributeBinder]);
                }
            }

            return services;
        }

        // ------------- 属性特性绑定（支持 ConfigurationKeyNameAttribute 与 JsonPropertyNameAttribute） --------------
        private static T BindUsingAttributes<T>(this IConfiguration config) where T : new()
        {
            var result = new T();
            BindObject(config, result);
            return result;
        }

        private static void BindObject(IConfiguration config, object instance)
        {
            var type = instance.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanWrite) continue;

                // 优先使用 ConfigurationKeyNameAttribute（框架原生），其次使用 JsonPropertyNameAttribute
                var keyAttr = prop.GetCustomAttribute<ConfigurationKeyNameAttribute>();
                var jsonAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                var explicitKey = keyAttr?.Name ?? jsonAttr?.Name;

                // 生成候选 key 列表
                var candidates = explicitKey != null
                    ? [explicitKey, prop.Name]
                    : GenerateNameCandidates(prop.Name);

                IConfigurationSection foundSection = null;
                foreach (var candidate in candidates)
                {
                    var section = config.GetSection(candidate);
                    if (section.Exists() || section.Value != null)
                    {
                        foundSection = section;
                        break;
                    }

                    // 尝试扁平 key（包含 '.')
                    var flattened = TryFindSectionByFlattenedKey(config, candidate);
                    if (flattened != null)
                    {
                        foundSection = flattened;
                        break;
                    }
                }

                // 若仍为空，则尝试直接用属性名对应子节（允许主配置使用 PascalCase）
                foundSection ??= config.GetSection(prop.Name);

                if (IsSimpleType(prop.PropertyType))
                {
                    var raw = foundSection?.Value;
                    if (raw != null)
                    {
                        try
                        {
                            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            var conv = TypeDescriptor.GetConverter(targetType);
                            var converted = conv.ConvertFromInvariantString(raw);
                            prop.SetValue(instance, converted);
                        }
                        catch
                        {
                            // 忽略转换错误；可改为记录或抛出
                        }
                    }
                }
                else
                {
                    var subInstance = Activator.CreateInstance(prop.PropertyType);
                    if (subInstance != null)
                    {
                        BindObject(foundSection ?? config.GetSection(prop.Name), subInstance);
                        prop.SetValue(instance, subInstance);
                    }
                }
            }
        }

        private static IConfigurationSection TryFindSectionByFlattenedKey(IConfiguration config, string key)
        {
            var s = config.GetSection(key);
            if (s.Exists() || s.Value != null) return s;

            var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1) return null;

            var cur = config;
            foreach (var part in parts)
            {
                cur = cur.GetSection(part);
            }

            return cur as IConfigurationSection;
        }

        private static string[] GenerateNameCandidates(string propName)
        {
            return new[]
            {
                propName, // PascalCase
                char.ToLowerInvariant(propName[0]) + propName.Substring(1), // camelCase
                ToSnakeCase(propName), // snake_case
                ToKebabCase(propName), // kebab-case
                ToDotNotation(propName) // dot.notation
            }.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        }

        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0) sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static string ToKebabCase(string name) => ToSnakeCase(name).Replace('_', '-');
        private static string ToDotNotation(string name) => ToSnakeCase(name).Replace('_', '.');

        private static bool IsSimpleType(Type t)
        {
            var type = Nullable.GetUnderlyingType(t) ?? t;
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(Guid)
                   || type == typeof(TimeSpan);
        }
    }
}