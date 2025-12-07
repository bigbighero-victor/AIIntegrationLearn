using VictorLearnAI.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureWebApplicationBuilder();

var app = builder.Build();

app.WebApplicationExtensions();

app.MapControllers();

app.Run();