using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Service.Indexing.Setter;
using Bug.Stu3Fhir.ResourceSupport;
using Hl7.Fhir.ElementModel;
using Hl7.FhirPath;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Indexing
{
  public class Indexer : IIndexer
  {
    private readonly ISearchParameterCache ISearchParameterCache;
    private readonly ITypedElementSupport ITypedElementSupport;
    private readonly IDateTimeSetterSupport IDateTimeSetterSupport;
    private readonly INumberSetterSupport INumberSetterSupport;
    private readonly IReferenceSetterSupport IReferenceSetterSupport;
    private readonly IStringSetterSupport IStringSetterSupport;
    private readonly ITokenSetterSupport ITokenSetterSupport;
    private readonly IQuantitySetterSupport IQuantitySetterSupport;
    private readonly IUriSetterSupport IUriSetterSupport;
    private readonly ILogger ILogger;

    public Indexer(ISearchParameterCache ISearchParameterCache,
      ITypedElementSupport ITypedElementSupport,
      IDateTimeSetterSupport IDateTimeSetterSupport,
      INumberSetterSupport INumberSetterSupport,
      IReferenceSetterSupport IReferenceSetterSupport,
      IStringSetterSupport IStringSetterSupport,
      ITokenSetterSupport ITokenSetterSupport,
      IQuantitySetterSupport IQuantitySetterSupport,
      IUriSetterSupport IUriSetterSupport,
      ILogger ILogger)
    {
      this.ISearchParameterCache = ISearchParameterCache;
      this.ITypedElementSupport = ITypedElementSupport;
      this.IDateTimeSetterSupport = IDateTimeSetterSupport;
      this.INumberSetterSupport = INumberSetterSupport;
      this.IReferenceSetterSupport = IReferenceSetterSupport;
      this.IStringSetterSupport = IStringSetterSupport;
      this.ITokenSetterSupport = ITokenSetterSupport;
      this.IQuantitySetterSupport = IQuantitySetterSupport;
      this.IUriSetterSupport = IUriSetterSupport;
      this.ILogger = ILogger;
    }

    public async Task<IndexerOutcome> Process(FhirResource fhirResource, Bug.Common.Enums.ResourceType resourceType)
    {

      var IndexerOutcome = new IndexerOutcome();

      List<SearchParameter> SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(fhirResource.FhirVersion, resourceType);


      foreach (SearchParameter SearchParameter in SearchParameterList)
      {
        //Composite searchParameters do not require populating as they are a Composite of other SearchParameter Types
        if (SearchParameter.SearchParamTypeId != Common.Enums.SearchParamType.Composite)
        {
          IEnumerable<ITypedElement>? ResultList;
          try
          {
            ResultList = ITypedElementSupport.Select(fhirResource, SearchParameter.FhirPath);
          } 
          catch (Exception Exec)
          {
            throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"Error indexing the FhirPath select expression for the SearchParameter with the name of : {SearchParameter.Name} for a resource type of : {resourceType.GetCode()} with the SearchParameter database primary key of {SearchParameter.Id.ToString()}.  The FhirPath expression was : {SearchParameter.FhirPath}. See inner exception for more info." , Exec);
          }

          //This null check of ResultList is only here due to the exception issue above.
          if (ResultList is object)
          {
            foreach (ITypedElement TypedElement in ResultList)
            {
              if (TypedElement != null)
              {
                switch (SearchParameter.SearchParamTypeId)
                {
                  case Common.Enums.SearchParamType.Number:
                    IndexerOutcome.QuantityIndexList.AddRange(INumberSetterSupport.Set(fhirResource.FhirVersion, TypedElement, resourceType, SearchParameter.Id, SearchParameter.Name));
                    break;
                  case Common.Enums.SearchParamType.Date:
                    IndexerOutcome.DateTimeIndexList.AddRange(IDateTimeSetterSupport.Set(fhirResource.FhirVersion, TypedElement, resourceType, SearchParameter.Id, SearchParameter.Name));

                    break;
                  case Common.Enums.SearchParamType.String:
                    IndexerOutcome.StringIndexList.AddRange(IStringSetterSupport.Set(fhirResource.FhirVersion, TypedElement, resourceType, SearchParameter.Id, SearchParameter.Name));
                    break;
                  case Common.Enums.SearchParamType.Token:
                    IndexerOutcome.TokenIndexList.AddRange(ITokenSetterSupport.Set(fhirResource.FhirVersion, TypedElement, resourceType, SearchParameter.Id, SearchParameter.Name));
                    break;
                  case Common.Enums.SearchParamType.Reference:
                    IndexerOutcome.ReferenceIndexList.AddRange(await IReferenceSetterSupport.SetAsync(fhirResource.FhirVersion, TypedElement, resourceType, SearchParameter.Id, SearchParameter.Name));
                    break;
                  case Common.Enums.SearchParamType.Composite:
                    //Composite searchParameters do not require populating as they are a Composite of other SearchParameter Types
                    break;
                  case Common.Enums.SearchParamType.Quantity:
                    IndexerOutcome.QuantityIndexList.AddRange(IQuantitySetterSupport.Set(fhirResource.FhirVersion, TypedElement, resourceType, SearchParameter.Id, SearchParameter.Name));
                    break;
                  case Common.Enums.SearchParamType.Uri:
                    IndexerOutcome.UriIndexList.AddRange(IUriSetterSupport.Set(fhirResource.FhirVersion, TypedElement, resourceType, SearchParameter.Id, SearchParameter.Name));
                    break;
                  case Common.Enums.SearchParamType.Special:
                    string SpecialMessage = $"Encountered a search parameter of type: {Common.Enums.SearchParamType.Special.GetCode()} which is not supported by the server. " +
                      $"The search parameter had the name of : {SearchParameter.Name} with a SearchParameter database primary key of {SearchParameter.Id.ToString()}. The " +
                      $"resource type being processed was of type : {resourceType.GetCode()}";
                    this.ILogger.LogWarning(SpecialMessage);
                    break;
                  default:
                    throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"Internal Server Error: Encountered an unknown SearchParamType of type {SearchParameter.SearchParamTypeId.GetCode()}");
                }
              }
            }
          }
        }
      }
      return IndexerOutcome;
    }
  }
}
