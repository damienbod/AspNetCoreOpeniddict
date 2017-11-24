using System;
using AspNetCoreResourceServer.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreResourceServer
{

    public class DataEventRecordContextFactory : IDesignTimeDbContextFactory<DataEventRecordContext>
    {
        public DataEventRecordContext CreateDbContext(string[] args)
        {
            var deploymentType = 
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Machine);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{deploymentType}.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<DataEventRecordContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DataEventRecordContext(optionsBuilder.Options);
        }
    }
}