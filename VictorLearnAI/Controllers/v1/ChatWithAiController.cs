using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using VictorLearnAI.AISupporters;

namespace VictorLearnAI.Web.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
public class ChatWithAiController(IAiSupporterService aiSupporterService): ControllerBase
{
    [HttpPost]
    public async Task<ContentResult> Post()
    {
        await aiSupporterService.ChatWithAi();
        return new ContentResult
        {
            StatusCode = 200,
            Content =  "aa"
        };
    }
}