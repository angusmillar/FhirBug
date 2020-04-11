using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Query;
using Bug.Common.Enums;
using Bug.Logic.Query.FhirApi;
using Microsoft.Extensions.Logging;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.Update;

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
      if (query is null)
        throw new ArgumentNullException(paramName: nameof(query));

      int Length = 25;
      //########################## Query ################################################################
      if (query is FhirBaseApiQuery FhirApiQuery)
      {        
        if (FhirApiQuery.RequestUri is null)
          throw new ArgumentNullException(paramName: nameof(FhirApiQuery.RequestUri));

        ILogger.LogInformation(this.PadRight($"####: Request: {query.GetType().Name} :", 80, '#'));
        ILogger.LogInformation(this.Pad($" Request URL", $"({FhirApiQuery.Method.GetCode()}) {FhirApiQuery.RequestUri.OriginalString}", Length));        
        ILogger.LogInformation(this.Pad(" Correlation Id", FhirApiQuery.CorrelationId, Length));
        ILogger.LogInformation(this.Pad(" FHIR Version", FhirApiQuery.FhirVersion.GetCode(), Length));
        bool ResourceInBody = (query is CreateQuery || query is UpdateQuery);        
        ILogger.LogInformation(this.Pad(" Has Resource", ResourceInBody.ToString(), Length));                  
        ILogger.LogInformation(this.PadRight($" -- HEADERS ", Length, '-'));
        foreach (var Header in FhirApiQuery.Headers.HeaderDictionary)
        {
          ILogger.LogInformation(this.Pad($"    {Header.Key}", Header.Value, Length));          
        }      
      }

      //########################## Handle ##################################################################
      var result = await this.decorated.Handle(query);
      //####################################################################################################

      //########################## Response ################################################################
      if (result is FhirApiResult FhirApiResult)
      {
        ILogger.LogInformation(this.PadRight($"####: Response: {((int)FhirApiResult.HttpStatusCode).ToString()} ({FhirApiResult.HttpStatusCode.ToString()})", 80, '#'));        
        ILogger.LogInformation(this.Pad(" Correlation Id", FhirApiResult.CorrelationId, Length));
        ILogger.LogInformation(this.Pad(" FHIR Version", FhirApiResult.FhirVersion.GetCode(), Length));
        
        if (!string.IsNullOrWhiteSpace(FhirApiResult.ResourceId))
        {
          ILogger.LogInformation(this.Pad(" Resource Id", FhirApiResult.ResourceId, Length));
        }
        if (FhirApiResult.VersionId.HasValue)
        {
          ILogger.LogInformation(this.Pad(" Version Id", FhirApiResult.VersionId.Value.ToString(), Length));
        }        
      }

      return result;
    }

    private string Pad(string parameter, string value, int totalPad)
    {
      return $"{this.PadRight($"{parameter}: ", totalPad, ' ')}{value}";
    }
    private string PadRight(string value, int total, char PadChar)
    {
      int cut = total - value.Length;
      return $"{value}{new String(PadChar, cut)}";
    }
  }
}
