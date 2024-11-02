using Microsoft.AspNetCore.Mvc;
using PersonalFinances.API.Secrets;

namespace PersonalFinances.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ISecretsManager secretsManager;

    public WeatherForecastController(ISecretsManager secretsManager)
    {
        this.secretsManager = secretsManager;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        var secret = await secretsManager.GetConnectionStringAsync();
        
        return Ok(secret);  
    }
}
