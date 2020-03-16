using DbUp;
using System;
using System.Linq;
using System.Reflection;

namespace Bug.DatabaseManager
{
  class Program
  {
    static int Main(string[] args)
    {
      string ProjectName = "Bug.DatabaseManager";
      var connectionString =
          args.FirstOrDefault()
          ?? "Host=localhost;Port=5432;Database=BugDb;Username=angusbmillar;Password=3agepufa";

      EnsureDatabase.For.PostgresqlDatabase(connectionString);

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
