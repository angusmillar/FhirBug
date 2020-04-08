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

namespace Bug.Logic.Service.SearchQuery
{
  public class SearchQueryService : ISearchQueryService
  {
    private readonly ISearchParameterCache ISearchParameterCache;
    private readonly ISearchQueryFactory ISearchQueryFactory;
    private readonly IOperationOutcomeSupport IOperationOutcomeSupport;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IKnownResource IKnownResource;
    private ResourceType ResourceContext { get; set; }
    private IFhirSearchQuery? SearchQuery { get; set; }
    private FhirVersion FhirVersion { get; set; }

    private SerachQueryServiceOutcome? Outcome { get; set; }
    public SearchQueryService(ISearchParameterCache ISearchParameterCache,
      ISearchQueryFactory ISearchQueryFactory,
      IOperationOutcomeSupport IOperationOutcomeSupport,
      IResourceTypeSupport IResourceTypeSupport,
      IKnownResource IKnownResource)
    {
      this.ISearchParameterCache = ISearchParameterCache;
      this.ISearchQueryFactory = ISearchQueryFactory;
      this.IOperationOutcomeSupport = IOperationOutcomeSupport;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IKnownResource = IKnownResource;
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
            await ChainSearchProcessing(Parameter);
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


    private async Task ChainSearchProcessing(KeyValuePair<string, StringValues> Parameter)
    {
      ISearchQueryBase? ParentChainSearchParameter = null;
      ISearchQueryBase? PreviousChainSearchParameter = null;
      string RawParameter = $"{Parameter.Key}={Parameter.Value}";
      bool ErrorInSearchParameterProcessing = false;
      List<InvalidSearchQueryParameter> InvalidSearchQueryParameterList = new List<InvalidSearchQueryParameter>();

      string[] ChaimedParameterSplit = Parameter.Key.Split(FhirSearchQuery.TermChainDelimiter);


      for (int i = 0; i < ChaimedParameterSplit.Length; i++)
      {
        Bug.Logic.DomainModel.SearchParameter? SearchParameter = null;
        string ParameterName = Parameter.Key.Split(FhirSearchQuery.TermChainDelimiter)[i];
        StringValues ParameterValue = string.Empty;
        //There is no valid Value for a chained reference parameter unless it is the last in a series 
        //of chains, so don't set it unless this is the last parameter in the whole chain.         
        if (i == ChaimedParameterSplit.Count() - 1)
          ParameterValue = Parameter.Value;

        var SingleChainedParameter = new KeyValuePair<string, StringValues>(ParameterName, ParameterValue);

        string ParameterNameNoModifier = string.Empty;
        string ParameterModifierTypedResource = string.Empty; 

        //Check for and deal with modifiers e.g 'Patient' in the example: subject:Patient.family=millar
        if (ParameterName.Contains(FhirSearchQuery.TermSearchModifierDelimiter))
        {
          string[] ParameterModifierSplit = ParameterName.Split(FhirSearchQuery.TermSearchModifierDelimiter);
          ParameterNameNoModifier = ParameterModifierSplit[0].Trim();

          if (ParameterModifierSplit.Length > 1)
          {
            ResourceType? ModifierResourceType = IResourceTypeSupport.GetTypeFromName(ParameterModifierSplit[1].Trim());
            if (ModifierResourceType.HasValue && IKnownResource.IsKnownResource(this.FhirVersion, ParameterModifierSplit[1].Trim()))
            {
              ParameterModifierTypedResource = ParameterModifierSplit[1].Trim();
            }
            else
            {
              ErrorInSearchParameterProcessing = true;
              InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(RawParameter, $"The resource type modifier: {ParameterModifierSplit[1].Trim()} within the chained search query of {RawParameter} is not a known resource for FHIR version: {this.FhirVersion.GetCode()} within this server"));
              break;
            }
          }
        }
        else
        {
          ParameterNameNoModifier = ParameterName;
        }

        List<Bug.Logic.DomainModel.SearchParameter> SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, this.ResourceContext);
        //Here we go through a series of ways to locate the SearchParameter for each segment of the chain query
        if (PreviousChainSearchParameter is null)
        {
          //If there is no previous then we look through the search parameter for the root resource type stored in this.ResourceContext
          SearchParameter = SearchParameterList.SingleOrDefault(x => x.Name == ParameterNameNoModifier);
          if (SearchParameter is null)
          {
            ErrorInSearchParameterProcessing = true;
            InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(RawParameter, $"The resource search parameter name of: {ParameterNameNoModifier} within the chained search query of: {RawParameter} is not a known search parameter for the resource: {this.ResourceContext} for this server in FHIR version: {this.FhirVersion.GetCode()}"));
            break;
          }
        }
        else
        {
          //Here we are using the PreviousChainSearchParameter's TypeModifierResource as the context to find the search parameter
          if (string.IsNullOrWhiteSpace(PreviousChainSearchParameter.TypeModifierResource))
          {
            //If there is no TypeModifierResource on the previous then we look at how many it supports and if only one we can use that.
            if (PreviousChainSearchParameter.TargetResourceTypeList.Count == 1)
            {
              PreviousChainSearchParameter.TypeModifierResource = PreviousChainSearchParameter.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode();
              List<Bug.Logic.DomainModel.SearchParameter> SearchParametersListForTarget = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, PreviousChainSearchParameter.TargetResourceTypeList.ToArray()[0].ResourceTypeId);              
              Bug.Logic.DomainModel.SearchParameter SearchParameterForTarget = SearchParametersListForTarget.SingleOrDefault(x => x.Name == ParameterNameNoModifier);              
              if (SearchParameterForTarget is null)
              {
                string Message = $"Unable to locate the search parameter named: {ParameterNameNoModifier} for the resource type: {PreviousChainSearchParameter.TypeModifierResource} for FHIR version: {this.FhirVersion.GetCode()} within the chain search query of: {RawParameter}";
                ErrorInSearchParameterProcessing = true;
                InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(RawParameter, Message));
                break;
              }
              else
              {
                SearchParameter = SearchParameterForTarget;
              }                        
            }
            else
            {
              //If more than one then we search for the given search parameter name among all  resource types supported for the PreviousChainSearchParameter
              Dictionary<ResourceType, Bug.Logic.DomainModel.SearchParameter> MultiChainedSearchParameter = new Dictionary<ResourceType, DomainModel.SearchParameter>();              
              foreach (var TargetResourceType in PreviousChainSearchParameter.TargetResourceTypeList)
              {
                List<Bug.Logic.DomainModel.SearchParameter> SearchParametersListForTarget = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, TargetResourceType.ResourceTypeId);                
                Bug.Logic.DomainModel.SearchParameter SearchParameterForTarget = SearchParametersListForTarget.SingleOrDefault(x => x.Name == ParameterNameNoModifier);
                if (SearchParameterForTarget != null)
                {
                  MultiChainedSearchParameter.Add(TargetResourceType.ResourceTypeId, SearchParameterForTarget);
                }
              }
              if (MultiChainedSearchParameter.Count() == 1)
              {
                //If this resolves to only one found then we use it
                PreviousChainSearchParameter.TypeModifierResource = MultiChainedSearchParameter.First().Key.GetCode();
                SearchParameter = MultiChainedSearchParameter.First().Value;
              }
              else
              {
                //We still have many to choose from so it cannot be resolved. The user need to specify the ResourceType with the Type modifier on the search parameter query e.g subject:Patient.family  
                string RefResources = string.Empty;
                foreach (var DicItem in MultiChainedSearchParameter)
                {
                  RefResources += ", " + DicItem.Key.GetCode();
                }
                string ResourceName = this.ResourceContext.GetCode();
                string Message = string.Empty;
                Message = $"The chained search parameter '{Parameter.Key}' is ambiguous. ";
                Message += $"Additional information: ";
                Message += $"The search parameter '{MultiChainedSearchParameter.First().Value.Name}' can reference the following resource types ({RefResources.TrimStart(',').Trim()}). ";
                Message += $"To correct this you must prefix the search parameter with a Type modifier, for example: '{PreviousChainSearchParameter.Name}:{MultiChainedSearchParameter.First().Key.GetCode()}.{MultiChainedSearchParameter.First().Value.Name}' ";
                Message += $"If the '{MultiChainedSearchParameter.First().Key.GetCode()}' resource was the intended reference for the search parameter '{MultiChainedSearchParameter.First().Value.Name}'.";                
                ErrorInSearchParameterProcessing = true;
                InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(RawParameter, Message));
                break;
              }
            }
          }
          else if (CheckModifierTypeResourceValidForSearchParameter(PreviousChainSearchParameter.TypeModifierResource, PreviousChainSearchParameter.TargetResourceTypeList))
          {
            //Double check the final Type modifier resource resolved is valid for the previous search parameter, the user could have got it wrong in the query.
            ResourceType ResourceTypeTest = IResourceTypeSupport.GetTypeFromName(PreviousChainSearchParameter.TypeModifierResource)!.Value;
            FhirVersion FhirVersionTest = this.FhirVersion;
            var TempSearchParameterList = await this.ISearchParameterCache.GetForIndexingAsync(FhirVersionTest, ResourceTypeTest);            
            SearchParameter = TempSearchParameterList.SingleOrDefault(x => x.Name == ParameterNameNoModifier);           
            PreviousChainSearchParameter.TypeModifierResource = PreviousChainSearchParameter.TypeModifierResource;
          }
          else
          {
            //The modifier target resource provided is not valid for the previous reference, e.g subject:DiagnosticReport.family=millar                        
            string ResourceName = this.ResourceContext.GetCode();
            string Message = $"The search parameter '{Parameter.Key}' is not supported by this server for the resource type '{ResourceName}'. ";
            Message += $"Additional information: ";
            Message += $"This search parameter was a chained search parameter. The part that was not recognized was '{PreviousChainSearchParameter.Name}.{ParameterName}', The search parameter modifier given '{PreviousChainSearchParameter.TypeModifierResource}' is not valid for the search parameter {PreviousChainSearchParameter.Name}. ";
            ErrorInSearchParameterProcessing = true;
            InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(RawParameter, Message));
            break;
          }
        }

        //We have no resolved a SearchParameter so we parse the value if it is the end of the chain, see IsChainedReferance 
        //or we are only parsing as a chain segment with no end value
        if (SearchParameter != null)
        {
          //If this is the last parameter in the chain then treat is as not a chain, otherwise treat as a chain
          bool IsChainedReferance = !(i >= ChaimedParameterSplit.Length - 1);
          IList<ISearchQueryBase> SearchQueryBaseList = await ISearchQueryFactory.Create(this.ResourceContext, SearchParameter, SingleChainedParameter, IsChainedReferance);                     
          
          foreach(ISearchQueryBase SearchQueryBase in SearchQueryBaseList)
          {            
            if (SearchQueryBase.IsValid)
            {
              if (SearchQueryBase.CloneDeep() is ISearchQueryBase SearchQueryBaseClone)
              {
                if (ParentChainSearchParameter is null)
                {
                  ParentChainSearchParameter = SearchQueryBaseClone;
                }
                else
                {                 
                  if (ParentChainSearchParameter.ChainedSearchParameter is null)
                  {
                    ParentChainSearchParameter.ChainedSearchParameter = SearchQueryBaseClone;
                  }

                  if (PreviousChainSearchParameter is object)
                  {
                    PreviousChainSearchParameter.ChainedSearchParameter = SearchQueryBaseClone;
                  }
                  else
                  {
                    throw new NullReferenceException(nameof(PreviousChainSearchParameter));
                  }
                }

                PreviousChainSearchParameter = SearchQueryBaseClone;
                if (i != ChaimedParameterSplit.Count() - 1)
                  PreviousChainSearchParameter.Modifier = SearchModifierCode.Type;
              }
              else
              {
                throw new InvalidCastException($"Internal Server Error: Unable to cast cloned SearchQueryBase to ISearchQueryBase");
              }
            }
            else
            {
              string Message = $"Failed to parse the value of the chain search query. Additional information: {SearchQueryBase.InvalidMessage}";
              ErrorInSearchParameterProcessing = true;
              InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(RawParameter, Message));
              break;
            }
          }          
        }
        else
        {
          ErrorInSearchParameterProcessing = true;
          InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(RawParameter, $"The resource search parameter name of: {ParameterNameNoModifier} within the chained search query of: {RawParameter} is not a known search parameter for the resource: {this.ResourceContext} for this server in FHIR version: {this.FhirVersion.GetCode()}"));
          break;
        }
      }


      //End of Chain loop


      if (!ErrorInSearchParameterProcessing)
      {
        if (ParentChainSearchParameter is object)
        {
          Outcome!.SearchQueryList.Add(ParentChainSearchParameter);
        }
        else
        {
          throw new NullReferenceException(nameof(PreviousChainSearchParameter));
        }
      }
      else
      {
        InvalidSearchQueryParameterList.ForEach(x => Outcome!.InvalidSearchQueryList.Add(x));        
      }
      
    }

        

    private bool CheckModifierTypeResourceValidForSearchParameter(string ModifierTypeResource, ICollection<DomainModel.SearchParameterTargetResourceType> TargetResourceTypeList)
    {
      if (TargetResourceTypeList == null)
        return false;
      return TargetResourceTypeList.Any(x => x.ResourceTypeId.GetCode() == ModifierTypeResource);
    }
  }
}
