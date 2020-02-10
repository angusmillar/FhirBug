using Bug.Logic.Command;
using Bug.Common.Enums;
using Bug.Logic.Command.FhirApi.Update;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Command.FhirApi.Create;

namespace Bug.Logic.Command.FhirApi
{
  public class FhirApiCommandHandler : ICommandHandler<FhirApiCommand, FhirApiOutcome> 
  {
    private readonly ILogger ILogger;

    public FhirApiCommandHandler(ILogger logger)
    {
      this.ILogger = logger;
    }

    public async Task<FhirApiOutcome> Handle(FhirApiCommand command)
    {
      ILogger.LogInformation($"FhirVersion: {command.FhirMajorVersion.GetLiteral()}");
      ILogger.LogInformation($"FhirVersion: {command.RequestUri.ToString()}");

      
      if (command is CreateCommand UpdateCommand)
      {

      }
      else if (command is UpdateCommand CreateCommand)
      {

      }      
      else
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"Unexpected FhirApiCommand of : {command.GetType().FullName}");
      }

      var OutCome = new FhirApiOutcome()
      {
        httpStatusCode = System.Net.HttpStatusCode.OK,        
      };
      return OutCome;
    }
  }
}
