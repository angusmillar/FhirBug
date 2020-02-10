using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.Json;
//using Microsoft.Extensions.Configuration.FileExtensions;
//using System;
//using System.Collections.Generic;
using System.IO;
//using System.Text;

namespace Bug.Data
{
  public class DesginTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
  {
    public AppDbContext CreateDbContext(string[] args)
    {
      IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../Bug.Api/appsettings.json").Build();
      var builder = new DbContextOptionsBuilder<AppDbContext>();
      var connectionString = config.GetConnectionString("MigrationDatabaseConnection");
      builder.UseNpgsql(connectionString);
      return new AppDbContext(builder.Options);
    }
  }
}
