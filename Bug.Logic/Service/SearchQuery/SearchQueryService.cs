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
      Outcome = new SerachQueryServiceOutcome(this.ResourceContext, searchQuery);
      //Parse Include and RevInclude parameters
      await ProcessIncludeSearchParameters(this.SearchQuery.Include);
      await ProcessIncludeSearchParameters(this.SearchQuery.RevInclude);

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
          Outcome!.InvalidSearchQueryList.Add(new InvalidSearchQueryParameter(Parameter.Key, ParamValue, Message));
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

          //All ok so add as valid include
          if (ParseOk)
          {            
            Outcome!.IncludeList.Add(SearchParameterInclude);
          }

        }
      }
    }

  }
}
