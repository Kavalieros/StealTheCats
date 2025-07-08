namespace StealTheCats.Dtos
{
    public class CatEntityFetchDto
    {
        public string? Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Url { get; set; }

        public List<BreedDto> Breeds { get; set; } = [];
    }

    public class BreedDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Temperament { get; set; }
    }
}
