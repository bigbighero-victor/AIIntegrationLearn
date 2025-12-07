using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace VictorLearnAI.Web.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
public class WeatherForecastController(AppSettings  appSettings) : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult Get()
    {
        return new ContentResult()
        {
            StatusCode = 200,
            Content =  JsonSerializer.Serialize(appSettings)
        };
    }
}