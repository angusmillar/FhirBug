using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Query;
using Bug.Common.Enums;
using Bug.Logic.Query.FhirApi;
using Microsoft.Extensions.Logging;

namespace Bug.Logic.Query.FhirApi.Decorator
{
  public class FhirApiQueryLoggingDecorator<TQuery, TResult> 
    : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>   
  {
    private readonly IQueryHandler<TQuery, TResult> decorated;
    private readonly ILogger ILogger;


    public FhirApiQueryLoggingDecorator(IQueryHandler<TQuery, TResult> decorated, ILogger logger)
    {
      this.decorated = decorated;
      this.ILogger = logger;
    }

    public async Task<TResult> Handle(TQuery query)
    {
      if (query is FhirApiQuery FhirApiQuery)
      {
        ILogger.LogInformation($"{new String('-', 80)}");
        ILogger.LogInformation($"Request Uri: {FhirApiQuery.RequestUriString.ToString()}");
        ILogger.LogInformation($"Major Fhir version: {FhirApiQuery.FhirMajorVersion.GetLiteral()}");
        //if (command is Bug.Logic.Commands.FhirApi.Update.UpdateCommand updateCommand)
        //{
        //  updateCommand.resource;
        //}
      }
      return await this.decorated.Handle(query);
    }
  }
}
