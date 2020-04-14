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
  public class IndexUriPredicateFactory : IIndexUriPredicateFactory
  {
    public Expression<Func<ResourceStore, bool>> UriIndex(SearchQueryUri SearchQueryUri)
    {
      var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);

      foreach (SearchQueryUriValue UriValue in SearchQueryUri.ValueList)
      {
        var IndexUriPredicate = LinqKit.PredicateBuilder.New<IndexUri>(true);
        IndexUriPredicate = IndexUriPredicate.And(IsSearchParameterId(SearchQueryUri.Id));

        if (!SearchQueryUri.Modifier.HasValue)
        {
          if (UriValue.Value is null)
          {
            throw new ArgumentNullException(nameof(UriValue.Value));
          }

          IndexUriPredicate = IndexUriPredicate.And(EqualTo(UriValue.Value.OriginalString));
          ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexUriPredicate));
        }
        else
        {
          var ArrayOfSupportedModifiers = Common.FhirTools.SearchQuery.SearchQuerySupport.GetModifiersForSearchType(SearchQueryUri.SearchParamTypeId);
          if (ArrayOfSupportedModifiers.Contains(SearchQueryUri.Modifier.Value))
          {
            if (SearchQueryUri.Modifier.Value != SearchModifierCode.Missing)
            {
              if (UriValue.Value is null)
              {
                throw new ArgumentNullException(nameof(UriValue.Value));
              }
            }
            switch (SearchQueryUri.Modifier.Value)
            {
              case SearchModifierCode.Missing:
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(IndexUriPredicate, !UriValue.IsMissing));
                break;
              case SearchModifierCode.Exact:
                IndexUriPredicate = IndexUriPredicate.And(EqualTo(UriValue.Value!.OriginalString));
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexUriPredicate));
                break;
              case SearchModifierCode.Contains:
                IndexUriPredicate = IndexUriPredicate.And(Contains(UriValue.Value!.OriginalString));
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexUriPredicate));
                break;
              case SearchModifierCode.Below:
                IndexUriPredicate = IndexUriPredicate.And(StartsWith(UriValue.Value!.OriginalString));
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexUriPredicate));
                break;
              case SearchModifierCode.Above:
                IndexUriPredicate = IndexUriPredicate.And(EndsWith(UriValue.Value!.OriginalString));
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexUriPredicate));
                break;
              default:
                throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryUri.Modifier.Value.GetCode()} has been added to the supported list for {SearchQueryUri.SearchParamTypeId.GetCode()} search parameter queries and yet no database predicate has been provided.");
            }
          }
          else
          {
            throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryUri.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryUri.SearchParamTypeId.GetCode()}.");
          }
        }
      }
      return ResourceStorePredicate;
    }

    private Expression<Func<ResourceStore, bool>> AnyIndex(Expression<Func<IndexUri, bool>> Predicate)
    {
      return x => x.UriIndexList.Any(Predicate.Compile());
    }
    private Expression<Func<ResourceStore, bool>> AnyIndexEquals(Expression<Func<IndexUri, bool>> Predicate, bool Equals)
    {
      return x => x.UriIndexList.Any(Predicate.Compile()) == Equals;
    }
    private Expression<Func<IndexUri, bool>> IsSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId == searchParameterId;
    }
    private Expression<Func<IndexUri, bool>> StartsWith(string value)
    {
      return x => x.Uri.StartsWith(value);
    }
    private Expression<Func<IndexUri, bool>> EndsWith(string value)
    {
      return x => x.Uri.EndsWith(value);
    }
    private Expression<Func<IndexUri, bool>> EqualTo(string Value)
    {
      return x => x.Uri.Equals(Value);
    }
    private Expression<Func<IndexUri, bool>> Contains(string Value)
    {
      return x => x.Uri.Contains(Value);
    }

  }
}
