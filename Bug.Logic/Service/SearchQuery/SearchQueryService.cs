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
using Bug.Common.FhirTools;
using Bug.Logic.Service.SearchQuery.ChainQuery;
using static Bug.Logic.Service.SearchQuery.Tools.FhirSearchQuery;

namespace Bug.Logic.Service.SearchQuery
{
  public class SearchQueryService : ISearchQueryService
  {
    private readonly ISearchParameterCache ISearchParameterCache;
    private readonly ISearchQueryFactory ISearchQueryFactory;   
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IKnownResource IKnownResource;
    private readonly IChainQueryProcessingService IChainQueryProcessingService;
    private ResourceType ResourceContext { get; set; }
    private IFhirSearchQuery? SearchQuery { get; set; }
    private FhirVersion FhirVersion { get; set; }


    private SerachQueryServiceOutcome? Outcome { get; set; }
    public SearchQueryService(ISearchParameterCache ISearchParameterCache,
      ISearchQueryFactory ISearchQueryFactory,      
      IResourceTypeSupport IResourceTypeSupport,
      IKnownResource IKnownResource,
      IChainQueryProcessingService IChainQueryProcessingService)
    {
      this.ISearchParameterCache = ISearchParameterCache;
      this.ISearchQueryFactory = ISearchQueryFactory;      
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IKnownResource = IKnownResource;
      this.IChainQueryProcessingService = IChainQueryProcessingService;
    }

    public async Task<ISerachQueryServiceOutcome> Process(FhirVersion fhirVersion, ResourceType resourceTypeContext, IFhirSearchQuery searchQuery)
    {
      this.FhirVersion = fhirVersion;
      this.ResourceContext = resourceTypeContext;
      this.SearchQuery = searchQuery;
      Outcome = new SerachQueryServiceOutcome(this.FhirVersion ,this.ResourceContext, searchQuery);
      //Parse Include and RevInclude parameters
      await ProcessIncludeSearchParameters(this.SearchQuery.Include);
      await ProcessIncludeSearchParameters(this.SearchQuery.RevInclude);
      await ProccessHasList(this.SearchQuery.Has);

      foreach (var Parameter in searchQuery.ParametersDictionary)
      {
        //We will just ignore an empty parameter such as this last '&' URL?family=Smith&given=John&
        if (Parameter.Key + Parameter.Value != string.Empty)
        {
          if (Parameter.Key.Contains(FhirSearchQuery.TermChainDelimiter))
          {
            ChainQueryProcessingOutcome ChainQueryProcessingOutcome = await IChainQueryProcessingService.Process(this.FhirVersion, this.ResourceContext, Parameter);
            ChainQueryProcessingOutcome.SearchQueryList.ForEach(x => Outcome.SearchQueryList.Add(x));
            ChainQueryProcessingOutcome.InvalidSearchQueryList.ForEach(x => Outcome.InvalidSearchQueryList.Add(x));
            ChainQueryProcessingOutcome.UnsupportedSearchQueryList.ForEach(x => Outcome.UnsupportedSearchQueryList.Add(x));
          }
          else
          {
            await NormalSearchProcessing(Parameter);
          }
        }
      }
      return Outcome;
    }

    private async Task ProccessHasList(IList<HasParameter> HasList)
    {
      foreach(var Has in HasList)
      {
        SearchQueryHas? Result = await ProccessHas(Has, Has.RawHasParameter);
        if (Result is object)
        {
          Outcome!.HasList.Add(Result);
        }
      }      
    }

    private async Task<SearchQueryHas?> ProccessHas(HasParameter Has, string RawHasParameter)
    {
      var Result = new SearchQueryHas();

      ResourceType? TargetResourceForSearchQuery = IResourceTypeSupport.GetTypeFromName(Has.TargetResourceForSearchQuery);
      if (TargetResourceForSearchQuery.HasValue && IKnownResource.IsKnownResource(this.FhirVersion, Has.TargetResourceForSearchQuery))
      {
        Result.TargetResourceForSearchQuery = TargetResourceForSearchQuery.Value;
      }
      else
      {
        Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawHasParameter, $"The resource type name of: {Has.TargetResourceForSearchQuery} in a {FhirSearchQuery.TermHas} parameter could not be resolved to a resource type supported by this server for FHIR version {this.FhirVersion.GetCode()}."));
        return null;
      }

      List<Bug.Logic.DomainModel.SearchParameter> SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, Result.TargetResourceForSearchQuery);
      Bug.Logic.DomainModel.SearchParameter BackReferenceSearchParameter = SearchParameterList.SingleOrDefault(x => x.Name == Has.BackReferenceSearchParameterName);
      if (BackReferenceSearchParameter is object && BackReferenceSearchParameter.SearchParamTypeId == SearchParamType.Reference)
      {
        Result.BackReferenceSearchParameter = BackReferenceSearchParameter;
      }
      else
      {
        if (BackReferenceSearchParameter is null)
        {
          string Message = $"The reference search parameter back to the target resource type of: {Has.BackReferenceSearchParameterName} is not a supported search parameter for the resource type {this.ResourceContext.GetCode()} for FHIR version {this.FhirVersion.GetCode()} within this server.";           
          Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawHasParameter, Message));
          return null;
        }
      }

      if (Has.ChildHasParameter is object)
      {
        Result.ChildSearchQueryHas = await ProccessHas(Has.ChildHasParameter, RawHasParameter);
        return Result;
      }
      else
      {
        if (Has.SearchQuery.HasValue)
        {
          SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, Result.TargetResourceForSearchQuery);
          Bug.Logic.DomainModel.SearchParameter SearchParameter = SearchParameterList.SingleOrDefault(x => x.Name == Has.SearchQuery.Value.Key);
          if (SearchParameter is object)
          {
            IList<ISearchQueryBase> SearchQueryBaseList = await ISearchQueryFactory.Create(this.ResourceContext, SearchParameter, Has.SearchQuery.Value);
            if (SearchQueryBaseList.Count == 1)
            {
              if (SearchQueryBaseList[0].IsValid)
              {
                Result.SearchQuery = SearchQueryBaseList[0];
                return Result;
              }
              else
              {
                string Message = $"Error parsing the search parameter found at the end of a {FhirSearchQuery.TermHas} query. The search parameter name was : {Has.SearchQuery.Value.Key} with the value of {Has.SearchQuery.Value.Value}. " +
                  $"Additional information: {SearchQueryBaseList[0].InvalidMessage}";
                Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawHasParameter, Message));
                return null;
              }
            }
            else
            {
              throw new ApplicationException($"The {FhirSearchQuery.TermHas} parameter seems to end with more then one search parameter, this should not be possible.");
            }           
          }
          else
          {
            string Message = $"The {FhirSearchQuery.TermHas} query finish with a search parameter: {Has.SearchQuery.Value.Key} for the resource type of: {Result.TargetResourceForSearchQuery.GetCode()}. " +
              $"However, the search parameter: {Has.SearchQuery.Value.Key} is not a supported search parameter for this resource type in this server for FHIR version {this.FhirVersion.GetCode()}.";
            Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawHasParameter, Message));
            return null;
          }
        }
        else
        {
          string Message = $"The {FhirSearchQuery.TermHas} query does not finish with a search parameter and value.";
          Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawHasParameter, Message));
          return null;
        }
      }
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
          if (SearchQueryBase.IsValid)
          {
            Outcome!.SearchQueryList.Add(SearchQueryBase);
          }
          else
          {
            Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(SearchQueryBase.RawValue, SearchQueryBase.InvalidMessage));
          }
        }
      }
      else
      {
        foreach (var ParamValue in Parameter.Value)
        {
          string Message = $"The search query parameter: {Parameter.Key} is not supported by this server for the resource type: {ResourceContext.GetCode()}, the whole parameter was : {Parameter.Key}={ParamValue}";
          Outcome!.UnsupportedSearchQueryList.Add(new InvalidSearchQueryParameter(Parameter.Key, ParamValue, Message));
        }
      }
    }

    private async Task ProcessIncludeSearchParameters(IList<IncludeParameter> IncludeList)
    {
      if (IncludeList != null)
      {
        foreach (var Include in IncludeList)
        {
          bool ParseOk = true;
          string RawParameter = $"{IncludeType.Include.GetCode()}:{FhirSearchQuery.TermIncludeRecurse}={Include.Value}";
          //if (this.FhirVersion == FhirVersion.Stu3 && Include.Recurse)
          //{
          //  if (Include.Type == IncludeType.Include)
          //  {
          //    string IncorrectRawParameter = $"{IncludeType.Include.GetCode()}:{FhirSearchQuery.TermIncludeRecurse}={Include.Value}";
          //    string CorrectRawParameter = $"{IncludeType.Include.GetCode()}:{FhirSearchQuery.TermIncludeRecurse}={Include.Value}";
          //    string Message = $"The Include type query of: {IncorrectRawParameter} is using the FHIR R4 {FhirSearchQuery.TermIncludeRecurse} yet this is a FHIR Stu3 search which must use {FhirSearchQuery.TermIncludeIterate} to achieve the same effect. " +
          //      $"Please update your query to use the FHIR Stu3 for such as: {CorrectRawParameter} ";              
          //    Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(IncorrectRawParameter, Message));
          //    ParseOk = false;
          //  }            
          //} 
          //else if (this.FhirVersion == FhirVersion.R4 && Include.Iterate)
          //{
          //  string IncorrectRawParameter = $"{IncludeType.Include.GetCode()}:{FhirSearchQuery.TermIncludeIterate}={Include.Value}";
          //  string CorrectRawParameter = $"{IncludeType.Include.GetCode()}:{FhirSearchQuery.TermIncludeRecurse}={Include.Value}";
          //  string Message = $"The Include type query of: {IncorrectRawParameter} is using the FHIR Stu3 {FhirSearchQuery.TermIncludeIterate} yet this is a FHIR R4 search which must use {FhirSearchQuery.TermIncludeRecurse} to achieve the same effect. " +
          //    $"Please update your query to use the FHIR R4 term such as: {CorrectRawParameter} ";
          //  Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(IncorrectRawParameter, Message));
          //  ParseOk = false;
          //}


          SearchQueryInclude SearchParameterInclude = new SearchQueryInclude(Include.Type)
          {
            IsIterate = Include.Iterate,
            IsRecurse = Include.Recurse
          };

          var valueSplitArray = Include.Value.Split(FhirSearchQuery.TermSearchModifierDelimiter);
          string ScourceResource = valueSplitArray[0].Trim();
          ResourceType? ScourceResourceType = IResourceTypeSupport.GetTypeFromName(ScourceResource);
          if (ScourceResourceType.HasValue && IKnownResource.IsKnownResource(this.FhirVersion, ScourceResource))          
          {
            SearchParameterInclude.SourceResourceType = ScourceResourceType.Value;
          }
          else
          {
            ParseOk = false;
            Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawParameter, $"The source resource type of: {ScourceResource} for the {Include.Type.GetCode()} parameter is not recognized as a FHIR {this.FhirVersion.GetCode()} resource type by this server."));
            break;
          }

          if (valueSplitArray.Count() > 2)
          {
            string TargetResourceTypeString = valueSplitArray[2].Trim();
            //checked we have a something if we don't then that is fine just a syntax error of the callers part 
            //i.e _includes=Patient:subject:
            if (!string.IsNullOrWhiteSpace(TargetResourceTypeString))
            {
              ResourceType? TargetResourceType = IResourceTypeSupport.GetTypeFromName(TargetResourceTypeString);
              if (TargetResourceType.HasValue && IKnownResource.IsKnownResource(this.FhirVersion, TargetResourceTypeString))               
              {
                SearchParameterInclude.SearchParameterTargetResourceType = TargetResourceType.Value;
              }
              else
              {
                ParseOk = false;
                Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawParameter, $"The target resource type of : {TargetResourceTypeString} for the {Include.Type.GetCode()} parameter is not recognized as a FHIR {this.FhirVersion.GetCode()} resource type by this server."));
                break;               
              }
            }
          }

          if (valueSplitArray.Count() > 1)
          {
            string SearchTerm = valueSplitArray[1].Trim();
            List<DomainModel.SearchParameter> SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, SearchParameterInclude.SourceResourceType.Value);            
            if (SearchTerm == "*")
            {
              if (SearchParameterInclude.SearchParameterTargetResourceType is object)
              {
                SearchParameterInclude.SearchParameterList = SearchParameterList.Where(x => x.SearchParamTypeId == SearchParamType.Reference && x.TargetResourceTypeList.Any(v => v.ResourceTypeId == SearchParameterInclude.SearchParameterTargetResourceType.Value)).ToList();
              }
              else
              {
                SearchParameterInclude.SearchParameterList = SearchParameterList.Where(x => x.SearchParamTypeId == SearchParamType.Reference).ToList();
              }
            }
            else
            {
              DomainModel.SearchParameter SearchParameter = SearchParameterList.SingleOrDefault(x => x.Name == SearchTerm);
              if (SearchParameter != null)
              {
                if (SearchParameter.SearchParamTypeId == SearchParamType.Reference)
                {
                  if (SearchParameter.TargetResourceTypeList != null && SearchParameterInclude.SearchParameterTargetResourceType.HasValue)
                  {
                    if (SearchParameter.TargetResourceTypeList.SingleOrDefault(x => x.ResourceTypeId == SearchParameterInclude.SearchParameterTargetResourceType.Value) == null)
                    {
                      ParseOk = false;
                      string Message = $"The target Resource '{SearchParameterInclude.SearchParameterTargetResourceType.Value.GetCode()}' of the _includes parameter is not recognized for the source '{SearchParameterInclude.SourceResourceType.GetCode()}' Resource's search parameter {SearchParameter.Name}.";
                      Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawParameter, Message));
                      break;                     
                    }
                  }
                  SearchParameterInclude.SearchParameterList = new List<DomainModel.SearchParameter>
                  {
                    SearchParameter
                  };
                }
                else
                {
                  ParseOk = false;
                  string Message = $"The source Resource '{SearchParameterInclude.SourceResourceType.GetCode()}' search parameter '{SearchParameter.Name}' of the _includes parameter is not of search parameter of type Reference, found search parameter type of '{SearchParameter.SearchParamTypeId.GetCode()}'.";
                  Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawParameter, Message));
                  break;                  
                }
              }
              else
              {
                ParseOk = false;
                string Message = $"The source Resource '{SearchParameterInclude.SourceResourceType.GetCode()}' search parameter '{SearchTerm}' is not a valid search parameter for the source Resource type.";
                Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(RawParameter, Message));
                break;
              }
            }
          }

          //All Ok so add as valid include
          if (ParseOk)
          {            
            Outcome!.IncludeList.Add(SearchParameterInclude);
          }

        }
      }
    }

  }
}
