using StealTheCats.Dtos;

namespace StealTheCats.Services
{
    public interface ICatService
    {
        Task CatsFetchJobAsync(int fetchCount = 25);
        Task<CatEntityFetchDto?> GetCatByIdAsync(int id);
        Task<List<CatEntityFetchDto>> GetCatsAsync(string? tag, int page, int pageSize);
    }
}
