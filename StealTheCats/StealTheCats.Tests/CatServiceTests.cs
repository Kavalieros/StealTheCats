using StealTheCats.Tests.Helpers;

namespace StealTheCats.Tests
{
    public class CatServiceTests
    {
        [Fact(DisplayName = "GetApiCatsAsync returns cats when API returns valid data")]
        public async Task GetApiCatsAsync()
        {
            // Arrange
            string jsonResponse = @"[
                {
                    ""id"": ""qg0_IodJp"",
                    ""url"": ""https://cdn2.thecatapi.com/images/qg0_IodJp.png"",
                    ""breeds"": []
                }
            ]";

            //string jsonResponse = @"[
            //    {""id"": ""qg0_IodJp"", ""url"": ""https://cdn2.thecatapi.com/images/qg0_IodJp.png"", ""breeds"": []},
            //    {""id"": ""A54VUs7Q6"", ""url"": ""https://cdn2.thecatapi.com/images/A54VUs7Q6.jpg"", ""breeds"": []},
            //    {""id"": ""IOqJ6RK7L"", ""url"": ""https://cdn2.thecatapi.com/images/IOqJ6RK7L.jpg"", ""breeds"": []},
            //    {""id"": ""-ATvkFxvA"", ""url"": ""https://cdn2.thecatapi.com/images/-ATvkFxvA.jpg"", ""breeds"": []}
            //]";

            var catService = CatServiceMockParams.CreateCatService(jsonResponse);

            // Act
            var result = await catService.GetApiCatsAsync(null, 2, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("qg0_IodJp", result[0].Id);
        }

        [Fact(DisplayName = "GetApiCatByIdAsync returns a cat requested by it's Id")]
        public async Task GetApiCatByIdAsync()
        {
            // Arrange
            string jsonResponse = @"
                {
                    ""id"": ""qg0_IodJp"",
                    ""url"": ""https://cdn2.thecatapi.com/images/qg0_IodJp.png"",
                    ""breeds"": []
                }
            ";

            var catService = CatServiceMockParams.CreateCatService(jsonResponse);

            // Act
            var result = await catService.GetApiCatByIdAsync("qg0_IodJp");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("qg0_IodJp", result!.Id);
        }
    }
}
