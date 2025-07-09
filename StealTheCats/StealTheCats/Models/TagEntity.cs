namespace StealTheCats.Models
{
    public class TagEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.Now;

        // many cats can share a tag
        public List<CatEntity> Cats { get; set; } = [];
    }
}
