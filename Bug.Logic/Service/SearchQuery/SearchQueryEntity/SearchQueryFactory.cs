using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.SearchQuery;
using Bug.Common.Interfaces;
using Bug.Common.StringTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.SearchQuery.Tools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryFactory : ISearchQueryFactory
  {
    private readonly IFhirUriFactory IFhirUriFactory;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly ISearchParameterCache ISearchParameterCache;
    private readonly IKnownResource IKnownResource;
    public SearchQueryFactory(IFhirUriFactory IFhirUriFactory, 
      IResourceTypeSupport IResourceTypeSupport, 
      ISearchParameterCache ISearchParameterCache,
      IKnownResource IKnownResource)
    {
      this.IFhirUriFactory = IFhirUriFactory;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.ISearchParameterCache = ISearchParameterCache;
      this.IKnownResource = IKnownResource;
    }

    public async Task<IList<ISearchQueryBase>> Create(Bug.Common.Enums.ResourceType ResourceContext, SearchParameter searchParameter, KeyValuePair<string, StringValues> Parameter, bool IsChainedReferance = false)
    {      
      var Result = new List<ISearchQueryBase>();
      string ParameterName = Parameter.Key.Trim();
      foreach (var ParameterValue in Parameter.Value)
      {
        string RawValue;
        if (IsChainedReferance)
          RawValue = $"{ParameterName}{FhirSearchQuery.TermChainDelimiter}";
        else
          RawValue = $"{ParameterName}={ParameterValue}";

        ISearchQueryBase SearchQueryBase = InitalizeSearchQueryEntity(searchParameter, ResourceContext, IsChainedReferance, RawValue);        
        Result.Add(SearchQueryBase);
        
        SearchQueryBase.ParseModifier(ParameterName, this.IResourceTypeSupport, this.IKnownResource);

        if (SearchQueryBase.Modifier == SearchModifierCode.Type && !IsChainedReferance)
        {
          if (!SearchQueryBase.TryParseValue($"{SearchQueryBase.TypeModifierResource}/{ParameterValue}"))
          {
            SearchQueryBase.IsValid = false;
          }
        }
        else if (SearchQueryBase.SearchParamTypeId == Common.Enums.SearchParamType.Composite)
        {
          if (SearchQueryBase is SearchQueryComposite SearchQueryComposite)
          {            
            List<Bug.Logic.DomainModel.SearchParameter> SearchParameterList = await ISearchParameterCache.GetForIndexingAsync(searchParameter.FhirVersionId, ResourceContext);
            List<ISearchQueryBase> SearchParameterBaseList = new List<ISearchQueryBase>();
            
            //Note we OrderBy SequentialOrder as they must be processed in this specific order
            foreach (SearchParameterComponent Component in SearchQueryComposite.ComponentList) //Should this be ordered by sentinel?
            {
              SearchParameter CompositeSearchParamter = SearchParameterList.SingleOrDefault(x =>  x.Url.IsEqualUri(Component.Definition));
              if (CompositeSearchParamter != null)
              {
                ISearchQueryBase CompositeSubSearchQueryBase = InitalizeSearchQueryEntity(CompositeSearchParamter, ResourceContext, false, RawValue);
                SearchParameterBaseList.Add(CompositeSubSearchQueryBase);
              }
              else
              {
                //This should not ever happen, but have message in case it does. We should never have a Composite
                //search parameter loaded like this as on load it is checked, but you never know!
                string Message =
                    $"Unable to locate one of the SearchParameters referenced in a Composite SearchParametrer type. " +
                    $"The Composite SearchParametrer Url was: {SearchQueryComposite.Url} for the resource type '{ResourceContext.GetCode()}'. " +
                    $"This SearchParamter references another SearchParamter with the Canonical Url of {Component.Definition}. " +
                    $"This SearchParamter can not be located in the FHIR Server. This is most likely a server error that will require investigation to resolve";
                SearchQueryComposite.InvalidMessage = Message;
                SearchQueryComposite.IsValid = false;
                break;               
              }
            }
            SearchQueryComposite.ParseCompositeValue(SearchParameterBaseList, ParameterValue);            
          }
          else
          {
            throw new InvalidCastException($"Unable to cast a {nameof(SearchQueryBase)} to {typeof(SearchQueryComposite).Name} when the {nameof(SearchQueryBase)}.{nameof(SearchQueryBase.SearchParamTypeId)} = {SearchQueryBase.SearchParamTypeId.GetCode()}");
          }
        }
        else
        {
          if (!SearchQueryBase.TryParseValue(ParameterValue))
          {
            SearchQueryBase.IsValid = false;
          }
        }
      }
      return Result;
    }

    private ISearchQueryBase InitalizeSearchQueryEntity(SearchParameter searchParameter, Bug.Common.Enums.ResourceType ResourceContext, bool IsChained, string RawValue)
    {
      switch (searchParameter.SearchParamTypeId)
      {
        case Common.Enums.SearchParamType.Number:
          return new SearchQueryNumber(searchParameter, ResourceContext, RawValue);
        case Common.Enums.SearchParamType.Date:
          return new SearchQueryDateTime(searchParameter, ResourceContext, RawValue);
        case Common.Enums.SearchParamType.String:
          return new SearchQueryString(searchParameter, ResourceContext, RawValue);
        case Common.Enums.SearchParamType.Token:
          return new SearchQueryToken(searchParameter, ResourceContext, RawValue);
        case Common.Enums.SearchParamType.Reference:
          return new SearchQueryReference(searchParameter, ResourceContext, this.IFhirUriFactory, RawValue, IsChained);
        case Common.Enums.SearchParamType.Composite:
          return new SearchQueryComposite(searchParameter, ResourceContext, RawValue);
        case Common.Enums.SearchParamType.Quantity:
          return new SearchQueryQuantity(searchParameter, ResourceContext, RawValue);
        case Common.Enums.SearchParamType.Uri:
          return new SearchQueryUri(searchParameter, ResourceContext, RawValue);
        case Common.Enums.SearchParamType.Special:
          return new SearchQueryNumber(searchParameter, ResourceContext, RawValue);
        default:
          throw new System.ComponentModel.InvalidEnumArgumentException(searchParameter.SearchParamTypeId.ToString(), (int)searchParameter.SearchParamTypeId, typeof(Bug.Common.Enums.SearchParamType));
      }
    }
    
  }
}
