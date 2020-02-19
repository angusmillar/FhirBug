using Bug.Logic.Query;
using Bug.Common.Enums;
using Bug.Logic.Query.FhirApi.Update;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Query.FhirApi.Create;

namespace Bug.Logic.Query.FhirApi
{
  public class FhirApiQueryHandler : IQueryHandler<FhirApiQuery, FhirApiResult> 
  {
    private readonly ILogger ILogger;

    public FhirApiQueryHandler(ILogger logger)
    {
      this.ILogger = logger;
    }

    public async Task<FhirApiResult> Handle(FhirApiQuery command)
    {
      ILogger.LogInformation($"FhirVersion: {command.FhirMajorVersion.GetLiteral()}");
      ILogger.LogInformation($"FhirVersion: {command.RequestUriString.ToString()}");

      
      if (command is CreateQuery UpdateCommand)
      {

      }
      else if (command is UpdateQuery CreateCommand)
      {

      }      
      else
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"Unexpected FhirApiCommand of : {command.GetType().FullName}");
      }

      var OutCome = new FhirApiResult()
      {
        HttpStatusCode = System.Net.HttpStatusCode.OK,        
      };
      return OutCome;
    }
  }
}
