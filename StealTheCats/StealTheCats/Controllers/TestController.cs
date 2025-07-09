using Microsoft.AspNetCore.Mvc;

namespace StealTheCats.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TestController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("apikey")]
        public IActionResult GetCatApiKey()
        {
            var apiKey = _configuration["TheCatApi:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                return NotFound(AppResources.ApiKeyNotFound);

            return Ok(AppResources.ApiKeyFound);
        }
    }
}
