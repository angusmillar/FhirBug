using Bug.Common.DecimalTools;
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
  public class IndexQuantityPredicateFactory : IIndexQuantityPredicateFactory
  {

    public Expression<Func<ResourceStore, bool>> QuantityIndex(SearchQueryQuantity SearchQueryQuantity)
    {
      var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);

      foreach (SearchQueryQuantityValue QuantityValue in SearchQueryQuantity.ValueList)
      {
        var IndexQuantityPredicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
        IndexQuantityPredicate = IndexQuantityPredicate.And(IsSearchParameterId(SearchQueryQuantity.Id));

        if (!SearchQueryQuantity.Modifier.HasValue)
        {
          if (!QuantityValue.Prefix.HasValue)
          {
            IndexQuantityPredicate = IndexQuantityPredicate.And(EqualTo(QuantityValue));
            ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
          }
          else
          {
            var ArrayOfSupportedPrefixes = Common.FhirTools.SearchQuery.SearchQuerySupport.GetPrefixListForSearchType(SearchQueryQuantity.SearchParamTypeId);
            if (ArrayOfSupportedPrefixes.Contains(QuantityValue.Prefix.Value))
            {
              switch (QuantityValue.Prefix.Value)
              {
                case SearchComparator.Eq:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(EqualTo(QuantityValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Ne:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(NotEqualTo(QuantityValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Gt:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(GreaterThan(QuantityValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Lt:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(LessThan(QuantityValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Ge:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(GreaterThanOrEqualTo(QuantityValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                case SearchComparator.Le:
                  IndexQuantityPredicate = IndexQuantityPredicate.And(LessThanOrEqualTo(QuantityValue));
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexQuantityPredicate));
                  break;
                default:
                  throw new System.ComponentModel.InvalidEnumArgumentException(QuantityValue.Prefix.Value.GetCode(), (int)QuantityValue.Prefix.Value, typeof(SearchComparator));
              }
            }
            else
            {
              string SupportedPrefixes = String.Join(',', ArrayOfSupportedPrefixes);
              throw new ApplicationException($"Internal Server Error: The search query prefix: {QuantityValue.Prefix.Value.GetCode()} is not supported for search parameter types of: {SearchQueryQuantity.SearchParamTypeId.GetCode()}. The supported prefixes are: {SupportedPrefixes}");
            }
          }
        }
        else
        {
          var ArrayOfSupportedModifiers = Common.FhirTools.SearchQuery.SearchQuerySupport.GetModifiersForSearchType(SearchQueryQuantity.SearchParamTypeId);
          if (ArrayOfSupportedModifiers.Contains(SearchQueryQuantity.Modifier.Value))
          {
            if (SearchQueryQuantity.Modifier.Value != SearchModifierCode.Missing)
            {
              if (QuantityValue.Value is null)
              {
                throw new ArgumentNullException(nameof(QuantityValue.Value));
              }
            }
            switch (SearchQueryQuantity.Modifier.Value)
            {
              case SearchModifierCode.Missing:
                if (QuantityValue.Prefix.HasValue == false)
                {
                  ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(IndexQuantityPredicate, !QuantityValue.IsMissing));
                }
                else
                {
                  throw new ApplicationException($"Internal Server Error: Encountered a Quantity Query with a: {SearchModifierCode.Missing.GetCode()} modifier and a prefix of {QuantityValue.Prefix!.GetCode()}. This should not happen as a missing parameter value must be only True or False with no prefix.");
                }
                break;
              default:
                throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryQuantity.Modifier.Value.GetCode()} has been added to the supported list for {SearchQueryQuantity.SearchParamTypeId.GetCode()} search parameter queries and yet no database predicate has been provided.");
            }
          }
          else
          {
            throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryQuantity.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryQuantity.SearchParamTypeId.GetCode()}.");
          }
        }
      }
      return ResourceStorePredicate;
    }

    private Expression<Func<IndexQuantity, bool>> EqualTo(SearchQueryQuantityValue QuantityValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (QuantityValue.Value.HasValue && QuantityValue.Scale.HasValue)
      {
        if (QuantityValue.Code is object)
        {
          Predicate = Predicate.And(SystemCodeOrCodeUnitEqualTo(QuantityValue.System, QuantityValue.Code));
          Predicate = Predicate.And(QuantityEqualTo(QuantityValue.Value.Value, QuantityValue.Scale.Value));
          return Predicate;
        }
        else
        {
          Predicate = Predicate.And(QuantityEqualTo(QuantityValue.Value.Value, QuantityValue.Scale.Value));
          return Predicate;
        }
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(QuantityValue)} property of {nameof(QuantityValue.Value)} was found to be null.");
      }
    }
    private Expression<Func<IndexQuantity, bool>> NotEqualTo(SearchQueryQuantityValue QuantityValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (QuantityValue.Value.HasValue && QuantityValue.Scale.HasValue)
      {
        if (QuantityValue.Code is object)
        {
          Predicate = Predicate.Or(SystemCodeOrCodeUnitNotEqualTo(QuantityValue.System, QuantityValue.Code));
          Predicate = Predicate.Or(QuantityNotEqualTo(QuantityValue.Value.Value, QuantityValue.Scale.Value));
          return Predicate;
        }
        else
        {
          Predicate = Predicate.Or(QuantityNotEqualTo(QuantityValue.Value.Value, QuantityValue.Scale.Value));
          return Predicate;
        }
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(QuantityValue)} property of {nameof(QuantityValue.Value)} was found to be null.");
      }
    }
    private Expression<Func<IndexQuantity, bool>> GreaterThan(SearchQueryQuantityValue QuantityValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (QuantityValue.Value.HasValue && QuantityValue.Scale.HasValue)
      {
        if (QuantityValue.Code is object)
        {
          Predicate = Predicate.And(SystemCodeOrCodeUnitEqualTo(QuantityValue.System, QuantityValue.Code));
          Predicate = Predicate.And(QuantityGreaterThen(QuantityValue.Value.Value));
          return Predicate;
        }
        else
        {
          Predicate = Predicate.And(QuantityGreaterThen(QuantityValue.Value.Value));
          return Predicate;
        }
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(QuantityValue)} property of {nameof(QuantityValue.Value)} was found to be null.");
      }
    }
    private Expression<Func<IndexQuantity, bool>> GreaterThanOrEqualTo(SearchQueryQuantityValue QuantityValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (QuantityValue.Value.HasValue && QuantityValue.Scale.HasValue)
      {
        if (QuantityValue.Code is object)
        {
          Predicate = Predicate.And(SystemCodeOrCodeUnitEqualTo(QuantityValue.System, QuantityValue.Code));
          Predicate = Predicate.And(QuantityGreaterThanOrEqualTo(QuantityValue.Value.Value));
          return Predicate;
        }
        else
        {
          Predicate = Predicate.And(QuantityGreaterThanOrEqualTo(QuantityValue.Value.Value));
          return Predicate;
        }
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(QuantityValue)} property of {nameof(QuantityValue.Value)} was found to be null.");
      }
    }
    private Expression<Func<IndexQuantity, bool>> LessThan(SearchQueryQuantityValue QuantityValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (QuantityValue.Value.HasValue && QuantityValue.Scale.HasValue)
      {
        if (QuantityValue.Code is object)
        {
          Predicate = Predicate.And(SystemCodeOrCodeUnitEqualTo(QuantityValue.System, QuantityValue.Code));
          Predicate = Predicate.And(QuantityLessThen(QuantityValue.Value.Value));
          return Predicate;
        }
        else
        {
          Predicate = Predicate.And(QuantityLessThen(QuantityValue.Value.Value));
          return Predicate;
        }
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(QuantityValue)} property of {nameof(QuantityValue.Value)} was found to be null.");
      }
    }
    private Expression<Func<IndexQuantity, bool>> LessThanOrEqualTo(SearchQueryQuantityValue QuantityValue)
    {
      var Predicate = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      if (QuantityValue.Value.HasValue && QuantityValue.Scale.HasValue)
      {
        if (QuantityValue.Code is object)
        {
          Predicate = Predicate.And(SystemCodeOrCodeUnitEqualTo(QuantityValue.System, QuantityValue.Code));
          Predicate = Predicate.And(QuantityLessThanOrEqualTo(QuantityValue.Value.Value));
          return Predicate;
        }
        else
        {
          Predicate = Predicate.And(QuantityLessThanOrEqualTo(QuantityValue.Value.Value));
          return Predicate;
        }
      }
      else
      {
        throw new ArgumentNullException($"Internal Server Error: The {nameof(QuantityValue)} property of {nameof(QuantityValue.Value)} was found to be null.");
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
    private Expression<Func<IndexQuantity, bool>> QuantityEqualTo(decimal midValue, int scale)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      var lowValue = DecimalSupport.CalculateLowNumber(midValue, scale);
      var highValue = DecimalSupport.CalculateHighNumber(midValue, scale);

      //PredicateOne: x => x.Quantity >= lowValue && x.Quantity <= highValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsHigherThanOrEqualTo(lowValue));
      PredicateOne = PredicateOne.And(IndexDecimal_IsLowerThanOrEqualTo(highValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Quantity <= midValue && x.Comparator == GreaterOrEqual  
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.And(IndexDecimal_IsLowerThanOrEqualTo(midValue));
      PredicateTwo = PredicateTwo.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateThree: x => x.Quantity >= midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateThree: x => x.Quantity > midValue && x.Comparator == LessOrEqual  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsHigherThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateThree: x => x.Quantity < midValue && x.Comparator == GreaterThan  
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
    private Expression<Func<IndexQuantity, bool>> QuantityNotEqualTo(decimal midValue, int scale)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      var lowValue = DecimalSupport.CalculateLowNumber(midValue, scale);
      var highValue = DecimalSupport.CalculateHighNumber(midValue, scale);

      //PredicateOne: x => (x.Quantity < lowValue || x.Quantity > highValue) && x.Comparator == null            
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      var SubOr = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      SubOr = SubOr.Or(IndexDecimal_IsLowerThan(lowValue));
      SubOr = SubOr.Or(IndexDecimal_IsHigherThan(highValue));
      PredicateOne = PredicateOne.And(SubOr);
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Quantity <= midValue && x.Comparator == GreaterOrEqual  
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.And(IndexDecimal_IsHigherThan(midValue));
      PredicateTwo = PredicateTwo.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateThree: x => x.Quantity >= midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      //PredicateThree: x => x.Quantity > midValue && x.Comparator == LessOrEqual  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsLowerThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateThree: x => x.Quantity < midValue && x.Comparator == GreaterThan  
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
    private Expression<Func<IndexQuantity, bool>> QuantityGreaterThen(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Quantity > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsHigherThan(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      //PredicateThree: x => x.Quantity > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThan(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateFour: x => x.Quantity > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsHigherThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }
    private Expression<Func<IndexQuantity, bool>> QuantityGreaterThanOrEqualTo(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Quantity > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      //PredicateThree: x => x.Quantity > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsHigherThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));

      //PredicateFour: x => x.Quantity > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsHigherThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.LessThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }
    private Expression<Func<IndexQuantity, bool>> QuantityLessThen(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Quantity > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsLowerThan(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessThan));

      //PredicateThree: x => x.Quantity > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsLowerThan(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateFour: x => x.Quantity > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsLowerThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }
    private Expression<Func<IndexQuantity, bool>> QuantityLessThanOrEqualTo(decimal midValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexQuantity>(true);

      //PredicateOne: x => x.Quantity > midValue && x.Comparator == null      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateOne = PredicateOne.And(IndexDecimal_IsLowerThanOrEqualTo(midValue));
      PredicateOne = PredicateOne.And(ComparatorIsNull());

      //PredicateTwo: x => x.Comparator == GreaterOrEqual || x.Comparator == GreaterThan 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessOrEqual));
      PredicateTwo = PredicateTwo.Or(ComparatorIsEqualTo(QuantityComparator.LessThan));

      //PredicateThree: x => x.Quantity > midValue && x.Comparator == LessOrEqual  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateThree = PredicateThree.And(IndexDecimal_IsLowerThanOrEqualTo(midValue));
      PredicateThree = PredicateThree.And(ComparatorIsEqualTo(QuantityComparator.GreaterOrEqual));

      //PredicateFour: x => x.Quantity > midValue && x.Comparator == LessThan  
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexQuantity>(true);
      PredicateFour = PredicateFour.And(IndexDecimal_IsLowerThan(midValue));
      PredicateFour = PredicateFour.And(ComparatorIsEqualTo(QuantityComparator.GreaterThan));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);

      return PredicateMain;
    }
    private Expression<Func<IndexQuantity, bool>> SystemCodeOrCodeUnitEqualTo(string? system, string code)
    {
      if (system is null)
      {
        return x => (x.Code == code) || (x.Unit == code);
      }
      else
      {
        return x => (x.System == system) && (x.Code == code);
      }
    }
    private Expression<Func<IndexQuantity, bool>> SystemCodeOrCodeUnitNotEqualTo(string? system, string code)
    {
      if (system is null)
      {
        return x => (x.Code != code) && (x.Unit != code);
      }
      else
      {
        return x => (x.System != system) || (x.Code != code);
      }
    }
    private Expression<Func<IndexQuantity, bool>> ComparatorIsNull()
    {
      return x => x.Comparator == null;
    }
    private Expression<Func<IndexQuantity, bool>> ComparatorIsEqualTo(QuantityComparator? QuantityComparator)
    {
      return x => x.Comparator == QuantityComparator;
    }
    private Expression<Func<IndexQuantity, bool>> IndexDecimal_IsHigherThanOrEqualTo(decimal value)
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
