using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StealTheCats.Dtos
{
    public class GetCatsQueryDto
    {
        [DefaultValue(0)]
        public int Page { get; set; } = 0;

        [DefaultValue(10)]
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;

        [StringLength(20, ErrorMessage = "Tag length cannot exceed 20 characters.")]
        public string? Tag { get; set; }
    }

    public class GetCatByIdDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; } = null!;
    }

    public class FetchCatsDto
    {
        [DefaultValue(25)]
        [Required(ErrorMessage = "Id is required.")]
        public int FetchCount { get; set; }
    }
}
