using Bug.Logic.Service.SearchQuery.Tools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;
using Bug.Logic.CacheService;
using System.Threading.Tasks;
using System.Linq;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Logic.Service.Fhir;

namespace Bug.Logic.Service.SearchQuery
{
  public class SearchQueryService : ISearchQueryService
  {
    private readonly ISearchParameterCache ISearchParameterCache;
    private readonly ISearchQueryFactory ISearchQueryFactory;
    private readonly IOperationOutcomeSupport IOperationOutcomeSupport;
    private ResourceType ResourceContext { get; set; }
    private IFhirSearchQuery? SearchQuery { get; set; }
    private FhirVersion FhirVersion { get; set; }

    private SerachQueryServiceOutcome? Outcome { get; set; }
    public SearchQueryService(ISearchParameterCache ISearchParameterCache,
      ISearchQueryFactory ISearchQueryFactory,
      IOperationOutcomeSupport IOperationOutcomeSupport)
    {
      this.ISearchParameterCache = ISearchParameterCache;
      this.ISearchQueryFactory = ISearchQueryFactory;
      this.IOperationOutcomeSupport = IOperationOutcomeSupport;
    }

    public async Task<SerachQueryServiceOutcome> Process(FhirVersion fhirVersion, ResourceType resourceTypeContext, IFhirSearchQuery searchQuery)
    {
      this.FhirVersion = fhirVersion;
      this.ResourceContext = resourceTypeContext;
      this.SearchQuery = searchQuery;
      Outcome = new SerachQueryServiceOutcome(this.ResourceContext, this.SearchQuery.Count, this.SearchQuery.SummaryType);

      foreach (var Parameter in searchQuery.ParametersDictionary)
      {
        //We will just ignore an empty parameter such as this last '&' URL?family=Smith&given=John&
        if (Parameter.Key + Parameter.Value != string.Empty)
        {
          if (Parameter.Key.Contains(FhirSearchQuery.TermChainDelimiter))
          {
            //ChainSearchProcessing(Parameter);
          }
          else
          {
            await NormalSearchProcessing(Parameter);
          }
        }
      }
      return Outcome;

    }

    private async Task NormalSearchProcessing(KeyValuePair<string, StringValues> Parameter)
    {
      List<Bug.Logic.DomainModel.SearchParameter> SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, this.ResourceContext);

      //Remove modifiers
      var SearchParameterName = Parameter.Key.Split(FhirSearchQuery.TermSearchModifierDelimiter)[0].Trim();
      Bug.Logic.DomainModel.SearchParameter SearchParameter = SearchParameterList.SingleOrDefault(x => x.Name == SearchParameterName);

      if (SearchParameter != null)
      {
        IList<ISearchQueryBase> SearchQueryBaseList = await ISearchQueryFactory.Create(this.ResourceContext, SearchParameter, Parameter);

        foreach (ISearchQueryBase SearchQueryBase in SearchQueryBaseList)
        {
          if (SearchQueryBase is SearchQueryReference SearchQueryReference)
          {
            var ErrorMessageList = new List<string>();
            foreach (var x in SearchQueryReference.ValueList)
            {
              if (x.FhirUri != null && string.IsNullOrWhiteSpace(x.FhirUri.ResourseName))
              {
                //After parsing the search parameter of type reference if there is no Resource Type for the reference
                //e.g no Patient is the example 'Patient/123456'
                string RefResources = string.Empty;
                SearchQueryReference.TargetResourceTypeList.ToList().ForEach(v => RefResources += ", " + v.ResourceTypeId.GetCode());
                string ResourceName = this.ResourceContext.GetCode();
                string Message = string.Empty;
                Message = $"The search parameter '{Parameter.Key}' is ambiguous. ";
                Message += $"Additional information: ";
                Message += $"The search parameter '{SearchQueryReference.Name}' can reference the following resource types ({RefResources.TrimStart(',').Trim()}). ";
                Message += $"To correct this you must prefix the search parameter with a Type modifier, for example: '{SearchQueryReference.Name}={SearchQueryReference.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()}/{x.FhirUri.ResourceId}' ";
                Message += $"or: '{SearchQueryReference.Name}:{SearchQueryReference.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()}={x.FhirUri.ResourceId}' ";
                Message += $"If the '{SearchQueryReference.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()}' resource was the intended reference for the search parameter '{SearchQueryReference.Name}'.";
                ErrorMessageList.Add(Message);
              }
            }
            if (ErrorMessageList.Count != 0)
            {
              Outcome!.OperationOutcome = IOperationOutcomeSupport.GetError(this.FhirVersion, ErrorMessageList.ToArray());
            }
          }

          if (!SearchQueryBase.IsValid)
          {            
            Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(SearchQueryBase.RawValue, SearchQueryBase.InvalidMessage));
          }
          else
          {
            Outcome!.SearchQueryList.Add(SearchQueryBase);
          }
        }
      }
      else
      {
        foreach(var ParamValue in Parameter.Value)
        {
          string Message = $"The search query parameter: {Parameter.Key} is not supported by this server for the resource type: {ResourceContext.GetCode()}, the whole parameter was : {Parameter.Key}={ParamValue}";          
          Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(Parameter.Key, ParamValue, Message));          
        }        
      }
    }

  }
}
