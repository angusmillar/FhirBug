using Bug.Common.Enums;
using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public class IndexReferencePredicateFactory : IIndexReferencePredicateFactory
  {
    private readonly IServiceBaseUrlCache IServiceBaseUrlCache;
    private readonly IResourceNameToTypeMap IResourceNameToTypeMap;
    public IndexReferencePredicateFactory(IServiceBaseUrlCache IServiceBaseUrlCache, IResourceNameToTypeMap IResourceNameToTypeMap)
    {
      this.IServiceBaseUrlCache = IServiceBaseUrlCache;
      this.IResourceNameToTypeMap = IResourceNameToTypeMap;
    }

    public async Task<List<Expression<Func<IndexReference, bool>>>> ReferenceIndex(SearchQueryReference SearchQueryReference)
    {
      //var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);
      var ResultList = new List<Expression<Func<IndexReference, bool>>>();
      //Improved Query when searching for ResourceIds for the same ResourceType and search parameter yet different ResourceIds.
      //It creates a SQL 'IN' cause instead of many 'OR' statements and should be more efficient.        
      //Heavily used in chain searching where we traverse many References. 
      //The 'Type' modifier is already resolved when the search parameter is parsed, so the SearchValue.FhirRequestUri.ResourseName is the correct Resource name at this stage
      if (SearchQueryReference.ValueList.Count > 1 && SearchQueryReference.ValueList.TrueForAll(x =>
                                                                                                !x.IsMissing &&
                                                                                                x.FhirUri!.IsRelativeToServer &&
                                                                                                x.FhirUri.ResourseName == SearchQueryReference.ValueList[0].FhirUri!.ResourseName &&
                                                                                                string.IsNullOrWhiteSpace(x.FhirUri.VersionId)))
      {
        Common.Interfaces.DomainModel.IServiceBaseUrl PrimaryServiceBaseUrl = await IServiceBaseUrlCache.GetPrimaryAsync(SearchQueryReference.FhirVersionId);
        var QuickIndexReferencePredicate = LinqKit.PredicateBuilder.New<IndexReference>(true);
        string[] ReferenceFhirIdArray = SearchQueryReference.ValueList.Select(x => x.FhirUri!.ResourceId).ToArray();
        QuickIndexReferencePredicate = QuickIndexReferencePredicate.And(IsSearchParameterId(SearchQueryReference.Id));
        QuickIndexReferencePredicate = QuickIndexReferencePredicate.And(EqualTo_ByKey_Many_ResourceIds(PrimaryServiceBaseUrl.Id,
                                                                                                      SearchQueryReference.ValueList[0].FhirUri!.ResourseName,
                                                                                                      ReferenceFhirIdArray,
                                                                                                      SearchQueryReference.ValueList[0].FhirUri!.VersionId));
        ResultList.Add(QuickIndexReferencePredicate);
        //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(QuickIndexReferencePredicate));
        return ResultList;
      }

      foreach (SearchQueryReferenceValue ReferenceValue in SearchQueryReference.ValueList)
      {
        var IndexReferencePredicate = LinqKit.PredicateBuilder.New<IndexReference>(true);
        IndexReferencePredicate = IndexReferencePredicate.And(IsSearchParameterId(SearchQueryReference.Id));

        if (!SearchQueryReference.Modifier.HasValue)
        {
          if (ReferenceValue.FhirUri is object)
          {
            if (ReferenceValue.FhirUri.IsRelativeToServer)
            {
              Common.Interfaces.DomainModel.IServiceBaseUrl PrimaryServiceBaseUrl = await IServiceBaseUrlCache.GetPrimaryAsync(SearchQueryReference.FhirVersionId);
              IndexReferencePredicate = IndexReferencePredicate.And(EqualTo_ByKey(PrimaryServiceBaseUrl.Id, ReferenceValue.FhirUri.ResourseName, ReferenceValue.FhirUri.ResourceId, ReferenceValue.FhirUri.VersionId));
              ResultList.Add(IndexReferencePredicate);
              //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexReferencePredicate));
            }
            else
            {
              IndexReferencePredicate = IndexReferencePredicate.And(EqualTo_ByUrlString(ReferenceValue.FhirUri.PrimaryServiceRootRemote!.OriginalString, ReferenceValue.FhirUri.ResourseName, ReferenceValue.FhirUri.ResourceId, ReferenceValue.FhirUri.VersionId));
              ResultList.Add(IndexReferencePredicate);
              //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexReferencePredicate));
            }
          }
          else
          {
            throw new ArgumentNullException(nameof(ReferenceValue.FhirUri));
          }
        }
        else
        {
          var ArrayOfSupportedModifiers = Common.FhirTools.SearchQuery.SearchQuerySupport.GetModifiersForSearchType(SearchQueryReference.SearchParamTypeId);
          if (ArrayOfSupportedModifiers.Contains(SearchQueryReference.Modifier.Value))
          {
            switch (SearchQueryReference.Modifier.Value)
            {
              case SearchModifierCode.Missing:
                IndexReferencePredicate = IndexReferencePredicate.And(IsNotSearchParameterId(SearchQueryReference.Id));
                ResultList.Add(IndexReferencePredicate);
                //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(IndexReferencePredicate, !ReferenceValue.IsMissing));
                break;
              default:
                throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryReference.Modifier.Value.GetCode()} has been added to the supported list for {SearchQueryReference.SearchParamTypeId.GetCode()} search parameter queries and yet no database predicate has been provided.");
            }
          }
          else
          {
            throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryReference.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryReference.SearchParamTypeId.GetCode()}.");
          }
        }

      }
      return ResultList;
    }

    private Expression<Func<IndexReference, bool>> EqualTo_ByKey_Many_ResourceIds(int PrimaryServiceBaseUrlId, string ResourceName, string[] ResourceIdArray, string VersionId)
    {      
      string? NullableVersionId = NullIfEmptyString(VersionId);
      Bug.Common.Enums.ResourceType ResourceType = GetResourceTypeFromString(ResourceName);
      return x => x.ServiceBaseUrlId == PrimaryServiceBaseUrlId && x.ResourceTypeId == ResourceType && ResourceIdArray.Contains(x.ResourceId) && x.VersionId == NullableVersionId;
    }

    private Expression<Func<IndexReference, bool>> EqualTo_ByKey(int PrimaryServiceBaseUrlId, string ResourceName, string ResourceId, string VersionId)
    {
      string? NullableVersionId = NullIfEmptyString(VersionId);
      Bug.Common.Enums.ResourceType ResourceType = GetResourceTypeFromString(ResourceName);
      return x => x.ServiceBaseUrlId == PrimaryServiceBaseUrlId && x.ResourceTypeId == ResourceType && x.ResourceId == ResourceId && x.VersionId == NullableVersionId;
    }

    private Expression<Func<IndexReference, bool>> EqualTo_ByUrlString(string RemoteServiceBaseUrl, string ResourceName, string ResourceId, string VersionId)
    {
      string? NullableVersionId = NullIfEmptyString(VersionId);
      Bug.Common.Enums.ResourceType ResourceType = GetResourceTypeFromString(ResourceName);
      RemoteServiceBaseUrl = Common.StringTools.StringSupport.StripHttp(RemoteServiceBaseUrl);
      return x => x.ServiceBaseUrl.Url == RemoteServiceBaseUrl && x.ResourceTypeId == ResourceType && x.ResourceId == ResourceId && x.VersionId == NullableVersionId;
    }


    private Expression<Func<ResourceStore, bool>> AnyIndex(Expression<Func<IndexReference, bool>> Predicate)
    {
      return x => x.ReferenceIndexList.Any(Predicate.Compile());
    }
    private Expression<Func<ResourceStore, bool>> AnyIndexEquals(Expression<Func<IndexReference, bool>> Predicate, bool Equals)
    {
      return x => x.ReferenceIndexList.Any(Predicate.Compile()) == Equals;
    }
    private Expression<Func<IndexReference, bool>> IsSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId == searchParameterId;
    }
    private Expression<Func<IndexReference, bool>> IsNotSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId != searchParameterId;
    }

    private string? NullIfEmptyString(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        return null;
      }
      else
      {
        return value;
      }
    }

    private Common.Enums.ResourceType GetResourceTypeFromString(string resourceName)
    {
      Common.Enums.ResourceType? ResourceType = IResourceNameToTypeMap.GetResourceType(resourceName);
      if (ResourceType.HasValue)
      {
        return ResourceType.Value;
      }
      else
      {
        throw new ApplicationException($"Internal Server Error: The {nameof(resourceName)} property equal to: {resourceName} from a {typeof(Common.Interfaces.IFhirUri).Name} was unable to be converted to a {typeof(Bug.Common.Enums.ResourceType).Name}");
      }
    }
  }
}
