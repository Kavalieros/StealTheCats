using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using StealTheCats.Services;

namespace StealTheCats.Tests.Helpers
{
    public static class CatServiceMockParams
    {
        public static CatService CreateCatService(string jsonResponse)
        {
            var httpClient = MockHttpClient.Create(jsonResponse);
            var dbContext = AppDbContextTest.CreateInMemoryDbContext();

            var inMemorySettings = new Dictionary<string, string?>
            {
                {"TheCatApi:ApiKey", "live_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var mockMapper = new Mock<IMapper>();

            return new CatService(httpClient, configuration, dbContext, mockMapper.Object);
        }
    }
}