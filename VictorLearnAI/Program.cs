using VictorLearnAI.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureWebApplicationBuilder()
    .AddServiceGroup(sc =>
    {
        //sc.AddScoped()
    })
    .ConfigureMapper(mapper =>
    {
        //mapper.AddMaps<aa, bb>();
    });

var app = builder.Build();

app.WebApplicationExtensions();

app.MapControllers();

app.Run();