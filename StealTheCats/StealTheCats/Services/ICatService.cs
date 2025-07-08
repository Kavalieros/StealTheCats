using StealTheCats.Dtos;
using StealTheCats.Models;

namespace StealTheCats.Services
{
    public interface ICatService
    {
        Task CatsFetchJobAsync(int fetchCount = 25);
        Task<CatEntityFetchDto?> GetApiCatByIdAsync(string catId);
        Task<CatEntity?> GetCatByIdAsync(string catId);
        Task<List<CatEntityFetchDto>?> GetApiCatsAsync(string? tag, int page, int pageSize);
        Task<List<CatEntity>?> GetCatsAsync(string? tag, int page, int pageSize);
    }
}
