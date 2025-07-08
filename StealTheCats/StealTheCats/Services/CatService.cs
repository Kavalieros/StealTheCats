using Microsoft.EntityFrameworkCore;
using StealTheCats.Data;
using StealTheCats.Dtos;
using StealTheCats.Helpers;

namespace StealTheCats.Services
{
    public class CatService : ICatService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        public CatService(HttpClient httpClient, IConfiguration configuration, AppDbContext dbContext)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-api-key", CatsUrlHelper.GetApiKey(configuration));
            _dbContext = dbContext;
        }

        public async Task CatsFetchJobAsync(int fetchCount = 25)
        {
            var catApiUrl = CatsUrlHelper.GetSearchUrl(limit: fetchCount);
            var catImages = await _httpClient.GetFromJsonAsync<List<CatEntityFetchDto>>(catApiUrl);

            if (catImages == null || catImages.Count == 0)
                return;

            foreach (var dto in catImages)
            {
                if (await _dbContext.Cats.AnyAsync(c => c.CatId == dto.Id))
                    continue;

                var imageBytes = await _httpClient.GetByteArrayAsync(dto.Url);

                var cat = CatMappingHelper.ToEntity(dto, imageBytes);

                foreach (var tag in cat.Tags)
                {
                    var existingTag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
                    if (existingTag != null)
                    {
                        tag.Id = existingTag.Id;
                        tag.Created = existingTag.Created;
                        tag.Cats = null!;
                    }
                }

                _dbContext.Cats.Add(cat);
            }

            await _dbContext.SaveChangesAsync();
        }

        public Task<CatEntityFetchDto?> GetCatByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CatEntityFetchDto>> GetCatsAsync(string? tag, int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
