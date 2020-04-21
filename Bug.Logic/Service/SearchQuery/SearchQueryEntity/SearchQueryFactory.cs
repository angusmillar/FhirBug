using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.SearchQuery;
using Bug.Common.Interfaces;
using Bug.Common.StringTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
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
    private readonly IFhirDateTimeFactory IFhirDateTimeFactory;
    private readonly ISearchParameterRepository ISearchParameterRepository;
    public SearchQueryFactory(IFhirUriFactory IFhirUriFactory, 
      IResourceTypeSupport IResourceTypeSupport, 
      ISearchParameterCache ISearchParameterCache,
      IKnownResource IKnownResource,
      IFhirDateTimeFactory IFhirDateTimeFactory,
      ISearchParameterRepository ISearchParameterRepository)
    {
      this.IFhirUriFactory = IFhirUriFactory;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.ISearchParameterCache = ISearchParameterCache;
      this.IKnownResource = IKnownResource;
      this.IFhirDateTimeFactory = IFhirDateTimeFactory;
      this.ISearchParameterRepository = ISearchParameterRepository;
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
          SearchQueryBase.ParseValue($"{SearchQueryBase.TypeModifierResource}/{ParameterValue}");          
        }
        else if (SearchQueryBase.SearchParamTypeId == Common.Enums.SearchParamType.Composite)
        {
          await LoadCompositeSubSearchParameters(ResourceContext, searchParameter, ParameterValue, RawValue, SearchQueryBase);
        }
        else
        {
          SearchQueryBase.ParseValue(ParameterValue);          
        }
      }
      return Result;
    }

    private async Task LoadCompositeSubSearchParameters(Common.Enums.ResourceType ResourceContext, SearchParameter searchParameter, string ParameterValue, string RawValue, ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryComposite SearchQueryComposite)
      {       
        List<ISearchQueryBase> SearchParameterBaseList = new List<ISearchQueryBase>();
        
        //Note we OrderBy SequentialOrder as they must be processed in this specific order
        foreach (SearchParameterComponent Component in SearchQueryComposite.ComponentList) //Should this be ordered by sentinel?
        {
          SearchParameter? CompositeSearchParamter = await ISearchParameterRepository.GetByCanonicalUrlAsync(SearchQueryComposite.FhirVersionId, ResourceContext, Component.Definition);          
          if (CompositeSearchParamter is object)
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

    private ISearchQueryBase InitalizeSearchQueryEntity(SearchParameter searchParameter, Bug.Common.Enums.ResourceType ResourceContext, bool IsChained, string RawValue)
    {
      return searchParameter.SearchParamTypeId switch
      {
        Common.Enums.SearchParamType.Number => new SearchQueryNumber(searchParameter, ResourceContext, RawValue),
        Common.Enums.SearchParamType.Date => new SearchQueryDateTime(searchParameter, ResourceContext, RawValue, IFhirDateTimeFactory),
        Common.Enums.SearchParamType.String => new SearchQueryString(searchParameter, ResourceContext, RawValue),
        Common.Enums.SearchParamType.Token => new SearchQueryToken(searchParameter, ResourceContext, RawValue),
        Common.Enums.SearchParamType.Reference => new SearchQueryReference(searchParameter, ResourceContext, this.IFhirUriFactory, RawValue, IsChained),
        Common.Enums.SearchParamType.Composite => new SearchQueryComposite(searchParameter, ResourceContext, RawValue),
        Common.Enums.SearchParamType.Quantity => new SearchQueryQuantity(searchParameter, ResourceContext, RawValue),
        Common.Enums.SearchParamType.Uri => new SearchQueryUri(searchParameter, ResourceContext, RawValue),
        Common.Enums.SearchParamType.Special => new SearchQueryNumber(searchParameter, ResourceContext, RawValue),
        _ => throw new System.ComponentModel.InvalidEnumArgumentException(searchParameter.SearchParamTypeId.ToString(), (int)searchParameter.SearchParamTypeId, typeof(Bug.Common.Enums.SearchParamType)),
      };
    }
    
  }
}
