using Bug.Common.Enums;
using Bug.Common.StringTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public class IndexTokenPredicateFactory : IIndexTokenPredicateFactory
  {
    public Expression<Func<ResourceStore, bool>> TokenIndex(SearchQueryToken SearchQueryToken)
    {
      var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);

      foreach (SearchQueryTokenValue TokenValue in SearchQueryToken.ValueList)
      {
        var IndexTokenPredicate = LinqKit.PredicateBuilder.New<IndexToken>(true);
        IndexTokenPredicate = IndexTokenPredicate.And(IsSearchParameterId(SearchQueryToken.Id));

        if (!SearchQueryToken.Modifier.HasValue)
        {
          IndexTokenPredicate = IndexTokenPredicate.And(EqualTo(TokenValue));
          ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexTokenPredicate));
        }
        else
        {
          var ArrayOfSupportedModifiers = Common.FhirTools.SearchQuery.SearchQuerySupport.GetModifiersForSearchType(SearchQueryToken.SearchParamTypeId);
          if (ArrayOfSupportedModifiers.Contains(SearchQueryToken.Modifier.Value))
          {
            switch (SearchQueryToken.Modifier.Value)
            {
              case SearchModifierCode.Missing:
                ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(IndexTokenPredicate, !TokenValue.IsMissing));
                break;
              default:
                throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryToken.Modifier.Value.GetCode()} has been added to the supported list for {SearchQueryToken.SearchParamTypeId.GetCode()} search parameter queries and yet no database predicate has been provided.");
            }
          }
          else
          {
            throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryToken.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryToken.SearchParamTypeId.GetCode()}.");
          }
        }
      }
      return ResourceStorePredicate;
    }

    private Expression<Func<ResourceStore, bool>> AnyIndex(Expression<Func<IndexToken, bool>> Predicate)
    {
      return x => x.TokenIndexList.Any(Predicate.Compile());
    }
    private Expression<Func<ResourceStore, bool>> AnyIndexEquals(Expression<Func<IndexToken, bool>> Predicate, bool Equals)
    {
      return x => x.TokenIndexList.Any(Predicate.Compile()) == Equals;
    }
    private Expression<Func<IndexToken, bool>> IsSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId == searchParameterId;
    }
    private Expression<Func<IndexToken, bool>> EqualTo(SearchQueryTokenValue TokenValue)
    {
      if (TokenValue.SearchType.HasValue)
      {
        string Code;
        string System;
        switch (TokenValue.SearchType.Value)
        {
          case SearchQueryTokenValue.TokenSearchType.MatchCodeOnly:
            Code = StringSupport.ToLowerFast(TokenValue.Code!);
            return x => x.Code == Code;
          case SearchQueryTokenValue.TokenSearchType.MatchSystemOnly:
            System = StringSupport.ToLowerFast(TokenValue.System!);
            return x => x.System == System;
          case SearchQueryTokenValue.TokenSearchType.MatchCodeAndSystem:
            System = StringSupport.ToLowerFast(TokenValue.System!);
            Code = StringSupport.ToLowerFast(TokenValue.Code!);
            return x => x.System == System && x.Code == Code;
          case SearchQueryTokenValue.TokenSearchType.MatchCodeWithNullSystem:
            Code = StringSupport.ToLowerFast(TokenValue.Code!);
            return x => x.System == null && x.Code == Code;
          default:
            throw new System.ComponentModel.InvalidEnumArgumentException(TokenValue.SearchType.Value.ToString(), (int)TokenValue.SearchType.Value, typeof(SearchQueryTokenValue.TokenSearchType));
        }
      }
      else
      {
        throw new ArgumentNullException(nameof(TokenValue.SearchType));
      }
    }

  }
}
