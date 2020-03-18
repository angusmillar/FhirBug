using DbUp;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bug.DatabaseManager
{
  class Program
  {
    static int Main(string[] args)
    {      
      string AppsettingsFilePath = Directory.GetCurrentDirectory().Replace(@"Bug.DatabaseManager\bin\Debug\netcoreapp3.1", @"Bug.Api\appsettings.json");      
      IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(AppsettingsFilePath).Build();      
      var DevelopmentConnectionString = config.GetConnectionString("MigrationDatabaseConnection");      
      var connectionString = args.FirstOrDefault() ?? DevelopmentConnectionString;

      EnsureDatabase.For.PostgresqlDatabase(connectionString);

      string ProjectName = "Bug.DatabaseManager";
      var upgrader =
          DeployChanges.To
              .PostgresqlDatabase(connectionString)
              .JournalToPostgresqlTable("public", "__DbUpMigrationHistory")
              .WithVariablesDisabled()
              .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith($"{ProjectName}.BeforeDeploymentScripts."), 
                new DbUp.Engine.SqlScriptOptions() { ScriptType = DbUp.Support.ScriptType.RunAlways, RunGroupOrder = 1 })
              .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith($"{ProjectName}.DeploymentScripts."),
                new DbUp.Engine.SqlScriptOptions() { ScriptType = DbUp.Support.ScriptType.RunAlways, RunGroupOrder = 2 })
              .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith($"{ProjectName}.PostDeploymentScripts."),
                new DbUp.Engine.SqlScriptOptions() { ScriptType = DbUp.Support.ScriptType.RunAlways, RunGroupOrder = 3 })
              .WithTransactionPerScript()
              .WithExecutionTimeout(TimeSpan.FromHours(1))
              .LogToConsole()
              .Build();
      
      var result = upgrader.PerformUpgrade();

      if (!result.Successful)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(result.Error);
        Console.ResetColor();
#if DEBUG
        Console.ReadLine();
#endif                
        return -1;
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Success!");
      Console.ResetColor();
      return 0;
    }
  }
}
