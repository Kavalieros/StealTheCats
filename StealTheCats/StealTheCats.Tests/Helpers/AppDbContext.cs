using Microsoft.EntityFrameworkCore;
using StealTheCats.Data;

namespace StealTheCats.Tests.Helpers
{
    public static class AppDbContextTest
    {
        public static AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            return new AppDbContext(options);
        }
    }
}
