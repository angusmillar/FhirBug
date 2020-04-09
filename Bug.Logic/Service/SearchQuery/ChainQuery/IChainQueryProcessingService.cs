using Bug.Common.Enums;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Service.SearchQuery.ChainQuery
{
  public interface IChainQueryProcessingService
  {
    Task<ChainQueryProcessingOutcome> Process(FhirVersion fhirVersion, ResourceType resourceContext, KeyValuePair<string, StringValues> Parameter);
  }
}