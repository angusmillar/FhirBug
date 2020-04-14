using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Bug.Data.Predicates
{
  public class IndexStringPredicateFactory : IIndexStringPredicateFactory
  {
    public Expression<Func<ResourceStore, bool>> StringIndex(SearchQueryString SearchQueryString)
    {
      var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);

      foreach (SearchQueryStringValue StringValue in SearchQueryString.ValueList)
      {
        var IndexStringPredicate = LinqKit.PredicateBuilder.New<IndexString>(true);
        IndexStringPredicate = IndexStringPredicate.And(IsSearchParameterId(SearchQueryString.Id));

        if (!SearchQueryString.Modifier.HasValue)
        {
          if (StringValue.Value is null)
          {
            throw new ArgumentNullException(nameof(StringValue.Value));
          }
          IndexStringPredicate = IndexStringPredicate.And(StartsWithOrEndsWith(StringValue.Value));
          ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexStringPredicate));
        }
        else
        {
          var ArrayOfSupportedModifiers = Common.FhirTools.SearchQuery.SearchQuerySupport.GetModifiersForSearchType(SearchQueryString.SearchParamTypeId);
          if (ArrayOfSupportedModifiers.Contains(SearchQueryString.Modifier.Value))
          {
            if (SearchQueryString.Modifier.Value != SearchModifierCode.Missing)
            {
              if (StringValue.Value is null)
              {
                throw new ArgumentNullException(nameof(StringValue.Value));
              }
            }

            switch (SearchQueryString.Modifier.Value)
            {
              case SearchModifierCode.Missing:
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(IndexStringPredicate, !StringValue.IsMissing));
                break;
              case SearchModifierCode.Exact:
                IndexStringPredicate = IndexStringPredicate.And(EqualTo(StringValue.Value!));
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexStringPredicate));
                break;
              case SearchModifierCode.Contains:
                IndexStringPredicate = IndexStringPredicate.And(Contains(StringValue.Value!));
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexStringPredicate));
                break;
              default:
                throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryString.Modifier.Value.GetCode()} has been added to the supported list for {SearchQueryString.SearchParamTypeId.GetCode()} search parameter queries and yet no database predicate has been provided.");
            }
          }
          else
          {
            throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryString.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryString.SearchParamTypeId.GetCode()}.");
          }
        }

      }
      return ResourceStorePredicate;
    }

    private Expression<Func<ResourceStore, bool>> AnyIndex(Expression<Func<IndexString, bool>> Predicate)
    {
      return x => x.StringIndexList.Any(Predicate.Compile());
    }
    private Expression<Func<ResourceStore, bool>> AnyIndexEquals(Expression<Func<IndexString, bool>> Predicate, bool Equals)
    {
      return x => x.StringIndexList.Any(Predicate.Compile()) == Equals;
    }
    private Expression<Func<IndexString, bool>> IsSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId == searchParameterId;
    }
    private Expression<Func<IndexString, bool>> StartsWithOrEndsWith(string stringValue)
    {
      return x => (x.String.StartsWith(stringValue) || x.String.EndsWith(stringValue));
    }
    private Expression<Func<IndexString, bool>> EqualTo(string stringValue)
    {
      return x => x.String.Equals(stringValue);
    }
    private Expression<Func<IndexString, bool>> Contains(string stringValue)
    {
      return x => x.String.Contains(stringValue);
    }

  }
}
