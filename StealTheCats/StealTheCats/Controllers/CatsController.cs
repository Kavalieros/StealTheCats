using Microsoft.AspNetCore.Mvc;
using StealTheCats.Dtos;
using StealTheCats.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace StealTheCats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController(ICatService catService) : ControllerBase
    {
        private readonly ICatService _catService = catService;

        [SwaggerOperation(
            Summary = "Start fetching cats asynchronously",
            Description = "Enqueues a background job to fetch cat images from the Cat API. Returns immediately with a job ID to track the status."
        )]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Fetch job started successfully")]
        [HttpPost("fetch")]
        public IActionResult FetchCatsAsync([FromQuery] FetchCatsDto dto)
        {
            var jobId = Hangfire.BackgroundJob.Enqueue(() => _catService.CatsFetchJobAsync(dto.FetchCount));
            return Accepted(new { JobId = jobId, Message = AppResources.FetchJobEnqueued });
        }

        [SwaggerOperation(
            Summary = "Get background job status",
            Description = "Retrieves the status and details of a background job by its job ID."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the status and details of the background job")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Job not found for the given ID")]
        [HttpGet("jobs/{id}")]
        public IActionResult GetJobStatus(string id)
        {
            if (!int.TryParse(id, out _))
                return NotFound(new { Message = AppResources.JobNotFound });

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
        [SwaggerOperation(
            Summary = "Get a cat image by ID",
            Description = "Retrieves a single cat image from the Cat API by its unique image ID."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the cat image details")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No cat found with the specified ID")]
        public async Task<IActionResult> GetApiCatByIdAsync([FromQuery] GetCatByIdDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cat = await _catService.GetApiCatByIdAsync(dto.Id);
            if (cat is null)
                return NotFound();

            return Ok(cat);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get paginated list of cat images",
            Description = "Retrieves a paginated list of cat images filtered optionally by a tag. Supports pagination via 'page' and 'pageSize' query parameters."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the list of cat images")]
        public async Task<IActionResult> GetCats([FromQuery] GetCatsQueryDto query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _catService.GetApiCatsAsync(query.Tag, query.Page, query.PageSize);
            return Ok(result);
        }
    }
}
