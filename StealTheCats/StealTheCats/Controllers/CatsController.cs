using Microsoft.AspNetCore.Mvc;
using StealTheCats.Services;

namespace StealTheCats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController(ICatService catService) : ControllerBase
    {
        private readonly ICatService _catService = catService;

        [HttpPost("fetch")]
        public IActionResult FetchCatsAsync(int fetchCount = 25)
        {
            var jobId = Hangfire.BackgroundJob.Enqueue(() => _catService.CatsFetchJobAsync(fetchCount));
            return Accepted(new { JobId = jobId, Message = AppResources.FetchJobEnqueued });
        }

        [HttpGet("jobs/{id}")]
        public IActionResult GetJobStatus(string id)
        {
            var monitoringApi = Hangfire.JobStorage.Current.GetMonitoringApi();
            var jobDetails = monitoringApi.JobDetails(id);

            if (jobDetails == null)
                return NotFound(new { Message = AppResources.JobNotFound });

            var lastState = jobDetails.History.FirstOrDefault();
            string? errorMessage = null;

            if (lastState?.Data != null && lastState.Data.TryGetValue("ExceptionMessage", out var exceptionMessage))
            {
                errorMessage = exceptionMessage;
            }

            return Ok(new
            {
                JobId = id,
                Status = lastState?.StateName,
                jobDetails.CreatedAt,
                ErrorMessage = errorMessage
            });
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetApiCatByIdAsync(string id)
        {
            var cat = await _catService.GetApiCatByIdAsync(id);
            if (cat is null)
                return NotFound();

            return Ok(cat);
        }

        //[HttpGet("id")]
        //public async Task<IActionResult> GetCatByIdAsync(string id)
        //{
        //    var cat = await _catService.GetCatByIdAsync(id);
        //    if (cat is null)
        //        return NotFound();

        //    return Ok(cat);
        //}

        [HttpGet]
        public async Task<IActionResult> GetCats([FromQuery] string? tag, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _catService.GetApiCatsAsync(tag, page, pageSize);
            return Ok(result);
        }
    }
}
