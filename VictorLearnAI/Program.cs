using VictorLearnAI.AISupporters;
using VictorLearnAI.AISupporters.Qianwen;
using VictorLearnAI.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureWebApplicationBuilder()
    .AddServiceGroup(sc =>
    {
        //sc.AddScoped()
        sc.AddScoped<IAiSupporterService, QwenClientHandler>();
    })
    .ConfigureMapper(mapper =>
    {
        //mapper.AddMaps<aa, bb>();
    });

var app = builder.Build();

app.WebApplicationExtensions();

app.MapControllers();

app.Run();