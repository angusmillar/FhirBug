using Bug.Common.DecimalTools;
using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public class IndexNumberPredicateFactory : IIndexNumberPredicateFactory
  {
    public Expression<Func<ResourceStore, bool>> NumberIndex(SearchQueryNumber SearchQueryNumber)
    {
      var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);

      foreach (SearchQueryNumberValue NumberValue in SearchQueryNumber.ValueList)
      {
        var IndexQuantityPredicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
        IndexQuantityPredicate = IndexQuantityPredicate.And(IsSearchParameterId(SearchQueryNumber.Id));

        if (!SearchQueryNumber.Modifier.HasValue)
        {
          if (!NumberValue.Prefix.HasValue)
          {
            IndexQuantityPredicate = IndexQuantityPredicate.And(EqualTo(NumberValue));
            ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
          }
          else
          {
            var ArrayOfSupportedPrefixes = Common.FhirTools.SearchQuery.SearchQuerySupport.GetPrefixListForSearchType(SearchQueryNumber.SearchParamTypeId);
            if (ArrayOfSupportedPrefixes.Contains(NumberValue.Prefix.Value))
            {
              switch (NumberValue.Prefix.Value)
              {
                case SearchComparator.Eq:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(EqualTo(NumberValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Ne:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(NotEqualTo(NumberValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Gt:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(GreaterThan(NumberValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Lt:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(LessThan(NumberValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Ge:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(GreaterThanOrEqualTo(NumberValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Le:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(LessThanOrEqualTo(NumberValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                default:
                  throw new System.ComponentModel.InvalidEnumArgumentException(NumberValue.Prefix.Value.GetCode(), (int)NumberValue.Prefix.Value, typeof(SearchComparator));
              }
            }
            else
            {
              string SupportedPrefixes = String.Join(',', ArrayOfSupportedPrefixes);
              throw new ApplicationException($"Internal Server Error: The search query prefix: {NumberValue.Prefix.Value.GetCode()} is not supported for search parameter types of: {SearchQueryNumber.SearchParamTypeId.GetCode()}. The supported prefixes are: {SupportedPrefixes}");
            }
          }
        }
        else
        {
          var ArrayOfSupportedModifiers = Common.FhirTools.SearchQuery.SearchQuerySupport.GetModifiersForSearchType(SearchQueryNumber.SearchParamTypeId);
          if (ArrayOfSupportedModifiers.Contains(SearchQueryNumber.Modifier.Value))
          {
            if (SearchQueryNumber.Modifier.Value != SearchModifierCode.Missing)
            {
              if (NumberValue.Value is null)
              {
                throw new ArgumentNullException(nameof(NumberValue.Value));
              }
            }
            switch (SearchQueryNumber.Modifier.Value)
            {
              case SearchModifierCode.Missing:
                if (NumberValue.Prefix.HasValue == false)
                {
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(IndexQuantityPredicate, !NumberValue.IsMissing));
                }
                else
                {
                  throw new ApplicationException($"Internal Server Error: Encountered a Number Query with a: {SearchModifierCode.Missing.GetCode()} modifier and a prefix of {NumberValue.Prefix!.GetCode()}. This should not happen as a missing parameter value must be only True or False with no prefix.");
                }
                break;
              default:
                throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryNumber.Modifier.Value.GetCode()} has been added to the supported list for {SearchQueryNumber.SearchParamTypeId.GetCode()} search parameter queries and yet no database predicate has been provided.");
            }
          }
          else
          {
            throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryNumber.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryNumber.SearchParamTypeId.GetCode()}.");
          }
        }
      }
      return ResourceStorePredicate;
    }

    private Expression<Func<IndexQuantity, bool>> EqualTo(SearchQueryNumberValue NumberValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (NumberValue.Value.HasValue && NumberValue.Scale.HasValue)
      {
        Predicate = Predicate.And(NumberEqualTo(NumberValue.Value.Value, NumberValue.Scale.Value));
        return Predicate;
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(NumberValue)} property of {nameof(NumberValue.Value)} was found to be null.");
      }
    }

    private Expression<Func<IndexQuantity, bool>> NotEqualTo(SearchQueryNumberValue NumberValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (NumberValue.Value.HasValue && NumberValue.Scale.HasValue)
      {
        Predicate = Predicate.And(NumberNotEqualTo(NumberValue.Value.Value, NumberValue.Scale.Value));
        return Predicate;
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(NumberValue)} property of {nameof(NumberValue.Value)} was found to be null.");
      }
    }

    private Expression<Func<IndexQuantity, bool>> GreaterThan(SearchQueryNumberValue NumberValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (NumberValue.Value.HasValue && NumberValue.Scale.HasValue)
      {
        Predicate = Predicate.And(NumberGreaterThen(NumberValue.Value.Value));
        return Predicate;
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(NumberValue)} property of {nameof(NumberValue.Value)} was found to be null.");
      }
    }

    private Expression<Func<IndexQuantity, bool>> GreaterThanOrEqualTo(SearchQueryNumberValue NumberValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (NumberValue.Value.HasValue && NumberValue.Scale.HasValue)
      {
        Predicate = Predicate.And(NumberGreaterThanOrEqualTo(NumberValue.Value.Value));
        return Predicate;
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(NumberValue)} property of {nameof(NumberValue.Value)} was found to be null.");
      }
    }

    private Expression<Func<IndexQuantity, bool>> LessThan(SearchQueryNumberValue NumberValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (NumberValue.Value.HasValue && NumberValue.Scale.HasValue)
      {
        Predicate = Predicate.And(NumberLessThen(NumberValue.Value.Value));
        return Predicate;
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(NumberValue)} property of {nameof(NumberValue.Value)} was found to be null.");
      }
    }

    private Expression<Func<IndexQuantity, bool>> LessThanOrEqualTo(SearchQueryNumberValue NumberValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (NumberValue.Value.HasValue && NumberValue.Scale.HasValue)
      {
        Predicate = Predicate.And(NumberLessThanOrEqualTo(NumberValue.Value.Value));
        return Predicate;
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(NumberValue)} property of {nameof(NumberValue.Value)} was found to be null.");
      }
    }


    private Expression<Func<ResourceStore, bool>> AnyIndex(Expression<Func<IndexQuantity, bool>> Predicate)
    {
      return x => x.QuantityIndexList.Any(Predicate.Compile());
    }
    private Expression<Func<ResourceStore, bool>> AnyIndexEquals(Expression<Func<IndexQuantity, bool>> Predicate, bool Equals)
    {
      return x => x.QuantityIndexList.Any(Predicate.Compile()) == Equals;
    }
    private Expression<Func<IndexQuantity, bool>> IsSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId == searchParameterId;
    }

    private Expression<Func<IndexQuantity, bool>> NumberEqualTo(decimal midValue, int scale)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      var lowValue = DecimalSupport.CalculateLowNumber(midValue, scale);
      var highValue = DecimalSupport.CalculateHighNumber(midValue, scale);

      //PredicateOne: x => x.Number >= lowValue && x.Number <= highValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsHigherThanOrEqualTo(lowValue));
      PredicateOne = PredicateOne.And(IndexDecimal_IsLowerThanOrEqualTo(highValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Number <= midValue && x.Comparator == GreaterOrEqual  
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.And(IndexDecimal_IsLowerThanOrEqualTo(midValue));
      PredicateTwo = PredicateTwo.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateThree: x => x.Number >= midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateThree: x => x.Number > midValue && x.Comparator == LessOrEqual  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsHigherThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateThree: x => x.Number < midValue && x.Comparator == GreaterThan  
      var PredicateFive = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFive = PredicateFive.And(IndexDecimal_IsLowerThan(midValue));
      PredicateFive = PredicateFive.And(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);
      PredicateMain = PredicateMain.Or(PredicateFive);

      return PredicateMain;
    }

    private Expression<Func<IndexQuantity, bool>> NumberNotEqualTo(decimal midValue, int scale)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      var lowValue = DecimalSupport.CalculateLowNumber(midValue, scale);
      var highValue = DecimalSupport.CalculateHighNumber(midValue, scale);

      //PredicateOne: x => (x.Number < lowValue || x.Number > highValue) && x.Comparator == null            
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      var SubOr = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      SubOr = SubOr.Or(IndexDecimal_IsLowerThan(lowValue));
      SubOr = SubOr.Or(IndexDecimal_IsHigherThan(highValue));
      PredicateOne = PredicateOne.And(SubOr);
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Number <= midValue && x.Comparator == GreaterOrEqual  
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.And(IndexDecimal_IsHigherThan(midValue));
      PredicateTwo = PredicateTwo.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateThree: x => x.Number >= midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      //PredicateThree: x => x.Number > midValue && x.Comparator == LessOrEqual  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsLowerThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateThree: x => x.Number < midValue && x.Comparator == GreaterThan  
      var PredicateFive = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFive = PredicateFive.And(IndexDecimal_IsLowerThanOrEqualTo(midValue));
      PredicateFive = PredicateFive.And(ComparatorIsEqualTo(QuantityComparator.LessThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);
      PredicateMain = PredicateMain.Or(PredicateFive);

      return PredicateMain;
    }

    private Expression<Func<IndexQuantity, bool>> NumberGreaterThen(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Number > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsHigherThan(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      //PredicateThree: x => x.Number > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThan(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateFour: x => x.Number > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsHigherThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }

    private Expression<Func<IndexQuantity, bool>> NumberGreaterThanOrEqualTo(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Number > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      //PredicateThree: x => x.Number > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateFour: x => x.Number > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsHigherThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }

    private Expression<Func<IndexQuantity, bool>> NumberLessThen(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Number > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsLowerThan(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessThan));

      //PredicateThree: x => x.Number > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsLowerThan(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateFour: x => x.Number > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsLowerThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }

    private Expression<Func<IndexQuantity, bool>> NumberLessThanOrEqualTo(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Number > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsLowerThanOrEqualTo(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessThan));

      //PredicateThree: x => x.Number > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsLowerThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateFour: x => x.Number > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsLowerThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }


    private Expression<Func<IndexQuantity, bool>> ComparatorIsNull()
    {
      return x => x.Comparator == null;
    }

    private Expression<Func<IndexQuantity, bool>> ComparatorIsEqualTo(QuantityComparator? QuantityComparator)
    {
      return x => x.Comparator == QuantityComparator;
    }

    private static Expression<Func<IndexQuantity, bool>> IndexDecimal_IsHigherThanOrEqualTo(decimal value)
    {
      return x => x.Quantity >= value;
    }

    private Expression<Func<IndexQuantity, bool>> IndexDecimal_IsHigherThan(decimal value)
    {
      return x => x.Quantity > value;
    }

    private Expression<Func<IndexQuantity, bool>> IndexDecimal_IsLowerThanOrEqualTo(decimal value)
    {
      return x => x.Quantity <= value;
    }

    private Expression<Func<IndexQuantity, bool>> IndexDecimal_IsLowerThan(decimal value)
    {
      return x => x.Quantity < value;
    }
  }
}
