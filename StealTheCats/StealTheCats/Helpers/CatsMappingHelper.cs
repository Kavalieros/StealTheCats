using StealTheCats.Dtos;
using StealTheCats.Models;

namespace StealTheCats.Helpers
{
    public static class CatMappingHelper
    {
        public static CatEntity ToEntity(CatEntityFetchDto dto, byte[] imageBytes)
        {
            var catEntity = new CatEntity
            {
                CatId = dto.Id!,
                Width = dto.Width,
                Height = dto.Height,
                Image = imageBytes,
                Created = DateTime.UtcNow,
                Tags = []
            };

            var temperamentTags = dto.Breeds
                .Where(b => !string.IsNullOrEmpty(b.Temperament))
                .SelectMany(b => b.Temperament!.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var tagName in temperamentTags)
            {
                catEntity.Tags.Add(new TagEntity
                {
                    Name = tagName,
                    Created = DateTime.Now
                });
            }

            return catEntity;
        }
    }

}
