using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StealTheCats.Data;
using StealTheCats.Dtos;
using StealTheCats.Helpers;
using StealTheCats.Models;

namespace StealTheCats.Services
{
    public class CatService : ICatService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CatService(HttpClient httpClient, IConfiguration configuration, AppDbContext dbContext, IMapper mapper)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-api-key", CatsUrlHelper.GetApiKey(configuration));
            _dbContext = dbContext;
            _mapper = mapper;
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

                var cat = MapDtoToEntity(dto, imageBytes);

                var resolvedTags = new List<TagEntity>();

                foreach (var tag in cat.Tags)
                {
                    var existingTag = await _dbContext.Tags
                        .FirstOrDefaultAsync(t => t.Name == tag.Name);

                    if (existingTag != null)
                        resolvedTags.Add(existingTag);
                    else
                        resolvedTags.Add(tag);
                }

                cat.Tags = resolvedTags;

                _dbContext.Cats.Add(cat);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<CatEntityFetchDto?> GetApiCatByIdAsync(string catId)
        {
            var catApiUrl = CatsUrlHelper.GetImagesUrl() + catId;

            try
            {
                return await _httpClient.GetFromJsonAsync<CatEntityFetchDto>(catApiUrl);
            }
            catch
            {
                return null;
            }
        }

        public async Task<CatEntity?> GetCatByIdAsync(string catId)
        {
            var catDto = await GetApiCatByIdAsync(catId);

            if (catDto == null)
                return null;

            var imageBytes = await _httpClient.GetByteArrayAsync(catDto.Url);

            var cat = MapDtoToEntity(catDto, imageBytes);

            var resolvedTags = new List<TagEntity>();

            foreach (var tag in cat.Tags)
            {
                var existingTag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.Name == tag.Name);

                if (existingTag != null)
                    resolvedTags.Add(existingTag);
                else
                    resolvedTags.Add(tag);
            }

            cat.Tags = resolvedTags;

            return cat;
        }

        public async Task<List<CatEntityFetchDto>?> GetApiCatsAsync(string? tag, int page, int pageSize)
        {
            if (page < 1) page = 1;
            var catApiUrl = $"{CatsUrlHelper.GetSearchUrl(pageSize)}&page={page - 1}&order=ASC";

            if (!string.IsNullOrEmpty(tag))
            {
                var breeds = await _httpClient.GetFromJsonAsync<List<BreedDto>?>(CatsUrlHelper.GetBreedsUrl());

                var matchingBreedIds = breeds?
                .Where(b => b.Temperament != null && b.Temperament.Contains(tag, StringComparison.OrdinalIgnoreCase))
                .Select(b => b.Id)
                .ToList();

                if (matchingBreedIds != null && matchingBreedIds.Count > 0)
                {
                    var breedIdsParam = string.Join(",", matchingBreedIds);
                    catApiUrl += $"&breed_ids={breedIdsParam}";
                }
            }

            var cats = await _httpClient.GetFromJsonAsync<List<CatEntityFetchDto>>(catApiUrl);
            return cats;
        }

        public CatEntity MapDtoToEntity(CatEntityFetchDto dto, byte[] imageBytes)
        {
            var catEntity = _mapper.Map<CatEntity>(dto);

            catEntity.Image = imageBytes;

            var temperamentTags = dto.Breeds?
                .Where(b => !string.IsNullOrEmpty(b.Temperament))
                .SelectMany(b => b.Temperament!.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? [];

            catEntity.Tags = [.. temperamentTags.Select(tagName => new TagEntity
            {
                Name = tagName,
                Created = DateTime.Now
            })];

            return catEntity;
        }

        public async Task<List<CatEntity>?> GetCatsAsync(string? tag, int page, int pageSize)
        {
            var catApiUrl = $"{CatsUrlHelper.GetImagesUrl()}search?page={page}&pageSize={pageSize}{(string.IsNullOrEmpty(tag) ? null : '&' + tag)}";

            var catImages = await GetApiCatsAsync(tag, page, pageSize);

            if (catImages == null || catImages.Count == 0)
                return null;

            var cats = new List<CatEntity>();

            foreach (var dto in catImages)
            {
                var imageBytes = await _httpClient.GetByteArrayAsync(dto.Url);

                var cat = MapDtoToEntity(dto, imageBytes);

                var resolvedTags = new List<TagEntity>();

                foreach (var cTag in cat.Tags)
                {
                    var existingTag = await _dbContext.Tags
                        .FirstOrDefaultAsync(t => t.Name == cTag.Name);

                    if (existingTag != null)
                        resolvedTags.Add(existingTag);
                    else
                        resolvedTags.Add(cTag);
                }

                cat.Tags = resolvedTags;
                cats.Add(cat);
            }
            return cats;
        }
    }
}
