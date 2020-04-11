using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Logic.Service.SearchQuery.Tools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Service.SearchQuery.ChainQuery
{
  public class ChainQueryProcessingService : IChainQueryProcessingService
  {
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IKnownResource IKnownResource;
    private readonly ISearchParameterCache ISearchParameterCache;
    private readonly ISearchQueryFactory ISearchQueryFactory;

    private FhirVersion FhirVersion;
    private ResourceType ResourceContext;
    private ISearchQueryBase? ParentChainSearchParameter;
    private ISearchQueryBase? PreviousChainSearchParameter;
    private bool ErrorInSearchParameterProcessing = false;
    private readonly List<InvalidSearchQueryParameter> InvalidSearchQueryParameterList;
    private readonly List<InvalidSearchQueryParameter> UnsupportedSearchQueryParameterList;
    private string RawParameter = string.Empty;

    public ChainQueryProcessingService(IResourceTypeSupport IResourceTypeSupport,
      IKnownResource IKnownResource,
      ISearchParameterCache ISearchParameterCache,
      ISearchQueryFactory ISearchQueryFactory)
    {
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IKnownResource = IKnownResource;
      this.ISearchParameterCache = ISearchParameterCache;
      this.ISearchQueryFactory = ISearchQueryFactory;

      this.InvalidSearchQueryParameterList = new List<InvalidSearchQueryParameter>();
      this.UnsupportedSearchQueryParameterList = new List<InvalidSearchQueryParameter>();
    }


    public async Task<ChainQueryProcessingOutcome> Process(FhirVersion fhirVersion, ResourceType resourceContext, KeyValuePair<string, StringValues> Parameter)
    {
      this.FhirVersion = fhirVersion;
      this.ResourceContext = resourceContext;

      var Outcome = new ChainQueryProcessingOutcome();
      this.RawParameter = $"{Parameter.Key}={Parameter.Value}";
      string[] ChaimedParameterSplit = Parameter.Key.Split(FhirSearchQuery.TermChainDelimiter);

      for (int i = 0; i < ChaimedParameterSplit.Length; i++)
      {
        //Each segment in the chain is a IsChainedReferance except the last segment in the chain which has the value.
        bool IsChainedReferance = !(i == ChaimedParameterSplit.Length - 1);
        Bug.Logic.DomainModel.SearchParameter? SearchParameter = null;
        string ParameterNameWithModifier = Parameter.Key.Split(FhirSearchQuery.TermChainDelimiter)[i];
        StringValues ParameterValue = string.Empty;
        //There is no valid Value for a chained reference parameter unless it is the last in a series 
        //of chains, so don't set it unless this is the last parameter in the whole chain.         
        if (i == ChaimedParameterSplit.Count() - 1)
          ParameterValue = Parameter.Value;

        var SingleChainedParameter = new KeyValuePair<string, StringValues>(ParameterNameWithModifier, ParameterValue);

        string ParameterName = string.Empty;
        string ParameterModifierTypedResource = string.Empty;

        //Check for and deal with modifiers e.g 'Patient' in the example: subject:Patient.family=millar
        if (ParameterNameWithModifier.Contains(FhirSearchQuery.TermSearchModifierDelimiter))
        {
          string[] ParameterModifierSplit = ParameterNameWithModifier.Split(FhirSearchQuery.TermSearchModifierDelimiter);
          ParameterName = ParameterModifierSplit[0].Trim();

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
              //If the Parent is ok then we can assume that any error further down the chain is an invalid search term rather than an unsupported term
              //as it is clear that this is a FHIR search term and not some other search parameter forgen to FHIR
              if (ParentChainSearchParameter is object)
              {
                InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, $"The resource type modifier: {ParameterModifierSplit[1].Trim()} within the chained search query of {this.RawParameter} is not a known resource for FHIR version: {this.FhirVersion.GetCode()} within this server"));
              }
              else
              {
                //Here we are only looking up the ParameterName to check weather this should be an unsupported parameter or an invalid parameter.
                //If we know the ParameterName then it is invalid whereas if we don't then it is unsupported and both are not known.
                List<Bug.Logic.DomainModel.SearchParameter> TempSearchParameterList = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, this.ResourceContext);
                var TempSearchParameter = TempSearchParameterList.SingleOrDefault(x => x.Name == ParameterName);
                if (TempSearchParameter is null)
                {
                  string Message = $"Both the search parameter name: {ParameterName} for the resource type: {this.ResourceContext.GetCode()} and its resource type modifier: {ParameterModifierSplit[1].Trim()} within the chained search query of {this.RawParameter} are unsupported within this server for FHIR version: {this.FhirVersion.GetCode()}.";
                  UnsupportedSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, Message));
                }
                else
                {
                  InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, $"The resource type modifier: {ParameterModifierSplit[1].Trim()} within the chained search query of {this.RawParameter} is not a known resource type for this server and FHIR version {this.FhirVersion.GetCode()}."));
                }
              }                            
              break;
            }
          }
        }
        else
        {
          ParameterName = ParameterNameWithModifier;
        }

        SearchParameter = await GetSearchParameter(ParameterName);

        //We have no resolved a SearchParameter so we parse the value if it is the end of the chain, see IsChainedReferance 
        //or we are only parsing as a chain segment with no end value
        if (SearchParameter != null)
        {          
          //If this is the last parameter in the chain then treat is as not a chain, otherwise treat as a chain         
          await SetChain(SearchParameter, SingleChainedParameter, IsChainedReferance);
        }
        else
        {
          ErrorInSearchParameterProcessing = true;
          if (this.InvalidSearchQueryParameterList.Count == 0 && this.UnsupportedSearchQueryParameterList.Count == 0)
          {
            throw new ApplicationException("Internal Server Error: When processing a chain search query we failed to resolve a search parameter for the query string however their are " +
              $"no items found in either the {nameof(InvalidSearchQueryParameterList)} or the {nameof(UnsupportedSearchQueryParameterList)}. This is an error in its self.");
          }          
          break;
        }
      }

      //End of Chain loop
      if (!ErrorInSearchParameterProcessing)
      {
        if (ParentChainSearchParameter is object)
        {
          Outcome!.SearchQueryList.Add(ParentChainSearchParameter);
          return Outcome;
        }
        else
        {
          throw new NullReferenceException(nameof(PreviousChainSearchParameter));
        }
      }
      else
      {
        InvalidSearchQueryParameterList.ForEach(x => Outcome!.InvalidSearchQueryList.Add(x));
        UnsupportedSearchQueryParameterList.ForEach(x => Outcome!.UnsupportedSearchQueryList.Add(x));
        return Outcome;
      }

    }

    private async Task SetChain(Bug.Logic.DomainModel.SearchParameter SearchParameter, KeyValuePair<string, StringValues> SingleChainedParameter,  bool IsChainedReferance)
    {
      IList<ISearchQueryBase> SearchQueryBaseList = await ISearchQueryFactory.Create(this.ResourceContext, SearchParameter, SingleChainedParameter, IsChainedReferance);

      foreach (ISearchQueryBase SearchQueryBase in SearchQueryBaseList)
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
            if (IsChainedReferance)
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
          InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, Message));
          break;
        }
      }
    }

    private async Task<Bug.Logic.DomainModel.SearchParameter?> GetSearchParameter(string parameterName)
    {
      Bug.Logic.DomainModel.SearchParameter SearchParameter;
      List<Bug.Logic.DomainModel.SearchParameter> SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, this.ResourceContext);
      //Here we go through a series of ways to locate the SearchParameter for each segment of the chain query
      if (PreviousChainSearchParameter is null)
      {
        //If there is no previous then we look through the search parameter for the root resource type stored in this.ResourceContext
        SearchParameter = SearchParameterList.SingleOrDefault(x => x.Name == parameterName);
        if (SearchParameter is null)
        {
          ErrorInSearchParameterProcessing = true;
          UnsupportedSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, $"The resource search parameter name of: {parameterName} within the chained search query of: {this.RawParameter} is not a known search parameter for the resource: {this.ResourceContext} for this server in FHIR version: {this.FhirVersion.GetCode()}"));
          return null;
        }
        else
        {
          return SearchParameter;
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
            Bug.Logic.DomainModel.SearchParameter SearchParameterForTarget = SearchParametersListForTarget.SingleOrDefault(x => x.Name == parameterName);
            if (SearchParameterForTarget is null)
            {
              string Message = $"Unable to locate the search parameter named: {parameterName} for the resource type: {PreviousChainSearchParameter.TypeModifierResource} for FHIR version: {this.FhirVersion.GetCode()} within the chain search query of: {this.RawParameter}";
              ErrorInSearchParameterProcessing = true;
              InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, Message));
              return null;
            }
            else
            {
              SearchParameter = SearchParameterForTarget;
              return SearchParameter;
            }
          }
          else
          {
            //If more than one then we search for the given search parameter name among all  resource types supported for the PreviousChainSearchParameter
            Dictionary<ResourceType, Bug.Logic.DomainModel.SearchParameter> MultiChainedSearchParameter = new Dictionary<ResourceType, DomainModel.SearchParameter>();
            foreach (var TargetResourceType in PreviousChainSearchParameter.TargetResourceTypeList)
            {
              List<Bug.Logic.DomainModel.SearchParameter> SearchParametersListForTarget = await ISearchParameterCache.GetForIndexingAsync(this.FhirVersion, TargetResourceType.ResourceTypeId);
              Bug.Logic.DomainModel.SearchParameter SearchParameterForTarget = SearchParametersListForTarget.SingleOrDefault(x => x.Name == parameterName);
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
              return SearchParameter;
            }
            else
            {
              if (MultiChainedSearchParameter.Count > 1)
              {
                //We still have many to choose from so it cannot be resolved. The user need to specify the ResourceType with the Type modifier on the search parameter query e.g subject:Patient.family  
                string RefResources = string.Empty;
                foreach (var DicItem in MultiChainedSearchParameter)
                {
                  RefResources += ", " + DicItem.Key.GetCode();
                }
                string ResourceName = this.ResourceContext.GetCode();
                string Message = string.Empty;
                Message = $"The chained search parameter '{this.RawParameter}' is ambiguous. ";
                Message += $"Additional information: ";
                Message += $"The search parameter '{parameterName}' could be a search parameter for any of the following resource types ({RefResources.TrimStart(',').Trim()}). ";
                Message += $"To correct this you must prefix the search parameter with a Type modifier, for example: '{PreviousChainSearchParameter.Name}:{MultiChainedSearchParameter.First().Key.GetCode()}.{MultiChainedSearchParameter.First().Value.Name}' ";
                Message += $"If the '{MultiChainedSearchParameter.First().Key.GetCode()}' resource was the intended reference for the search parameter '{MultiChainedSearchParameter.First().Value.Name}'.";
                ErrorInSearchParameterProcessing = true;
                InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, Message));
                return null;
              }
              else
              {
                //We have found zero matches for this search parameter name from the previous allowed resource types, so the search parameter name is possibly wrong.
                string TargetResourceTypes = string.Empty;
                foreach (var TargetResourceType in PreviousChainSearchParameter.TargetResourceTypeList)
                {
                  TargetResourceTypes += ", " + TargetResourceType.ResourceTypeId.GetCode();
                }
                string ResourceName = this.ResourceContext.GetCode();
                string Message = string.Empty;
                Message = $"The chained search parameter '{this.RawParameter}' is unresolvable. ";
                Message += $"Additional information: ";
                Message += $"The search parameter: {parameterName} should be a search parameter for any of the following resource types ({TargetResourceTypes.TrimStart(',').Trim()}) as resolved from the previous link in the chain: {PreviousChainSearchParameter.Name}. ";
                Message += $"To correct this you must specify a search parameter here that is supported by those resource types. ";
                Message += $"Please review your chained search query and specifically the use of the search parameter: {parameterName}'";
                ErrorInSearchParameterProcessing = true;
                InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, Message));
                return null;
              }                           
            }
          }
        }
        else if (CheckModifierTypeResourceValidForSearchParameter(PreviousChainSearchParameter.TypeModifierResource, PreviousChainSearchParameter.TargetResourceTypeList))
        {
          //PreviousChainSearchParameter.TypeModifierResource = PreviousChainSearchParameter.TypeModifierResource;
          //Double check the final Type modifier resource resolved is valid for the previous search parameter, the user could have got it wrong in the query.
          ResourceType ResourceTypeTest = IResourceTypeSupport.GetTypeFromName(PreviousChainSearchParameter.TypeModifierResource)!.Value;
          FhirVersion FhirVersionTest = this.FhirVersion;
          var TempSearchParameterList = await this.ISearchParameterCache.GetForIndexingAsync(FhirVersionTest, ResourceTypeTest);
          SearchParameter = TempSearchParameterList.SingleOrDefault(x => x.Name == parameterName);
          if (SearchParameter is object)
          {
            return SearchParameter;
          }
          else
          {
            string ResourceName = ResourceTypeTest.GetCode();
            string Message = $"The chained search query part: {parameterName} is not a supported search parameter name for the resource type: {ResourceName} for this server in FHIR version {this.FhirVersion.GetCode()}. ";
            Message += $"Additional information: ";
            Message += $"This search parameter was a chained search parameter. The part that was not recognized was: {parameterName}.";
            ErrorInSearchParameterProcessing = true;
            InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, Message));
            return null;
          }          
        }
        else
        {
          //The modifier target resource provided is not valid for the previous reference, e.g subject:DiagnosticReport.family=millar                        
          string ResourceName = this.ResourceContext.GetCode();
          string Message = $"The search parameter '{parameterName}' is not supported by this server for the resource type '{ResourceName}'. ";
          Message += $"Additional information: ";
          Message += $"This search parameter was a chained search parameter. The part that was not recognized was '{PreviousChainSearchParameter.Name}.{parameterName}', The search parameter modifier given '{PreviousChainSearchParameter.TypeModifierResource}' is not valid for the search parameter {PreviousChainSearchParameter.Name}. ";
          ErrorInSearchParameterProcessing = true;
          InvalidSearchQueryParameterList.Add(new InvalidSearchQueryParameter(this.RawParameter, Message));
          return null;
        }
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
