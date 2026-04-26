using AutoMapper;
using LearningPlatform.API.Data;
using LearningPlatform.API.Mappings;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Tests;

public static class TestHelpers
{
    public static AppDbContext CreateInMemoryDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AppDbContext(options);
    }

    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }
}
