using Bug.Common.DateTimeTools;
using Bug.Common.DecimalTools;
using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Bug.Data.Predicates
{
  public class IndexDateTimePredicateFactory : IIndexDateTimePredicateFactory
  {
    private readonly ISearchQueryCalcHighDateTime ISearchQueryCalcHighDateTime;
    public IndexDateTimePredicateFactory(ISearchQueryCalcHighDateTime ISearchQueryCalcHighDateTime)
    {
      this.ISearchQueryCalcHighDateTime = ISearchQueryCalcHighDateTime;
    }

    public List<Expression<Func<IndexDateTime, bool>>> DateTimeIndex(SearchQueryDateTime SearchQueryDateTime)
    {
      //var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);
      var ResultList = new List<Expression<Func<IndexDateTime, bool>>>();

      foreach (SearchQueryDateTimeValue DateTimeValue in SearchQueryDateTime.ValueList)
      {
        var IndexDateTimePredicate = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
        IndexDateTimePredicate = IndexDateTimePredicate.And(IsSearchParameterId(SearchQueryDateTime.Id));

        if (!SearchQueryDateTime.Modifier.HasValue)
        {
          if (!DateTimeValue.Value.HasValue || !DateTimeValue.Precision.HasValue)
          {
            throw new ArgumentNullException($"Internal Server Error: Either the {nameof(DateTimeValue.Value)} or the {nameof(DateTimeValue.Precision)} of a DateTime SearchQuery is null and yet there is no missing modifier.");
          }

          if (!DateTimeValue.Prefix.HasValue)
          {
            IndexDateTimePredicate = IndexDateTimePredicate.And(EqualTo(DateTimeValue.Value.Value, ISearchQueryCalcHighDateTime.SearchQueryCalculateHighDateTimeForRange(DateTimeValue.Value.Value, DateTimeValue.Precision.Value)));
            ResultList.Add(IndexDateTimePredicate);
            //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexDateTimePredicate));
          }
          else
          {
            var ArrayOfSupportedPrefixes = Common.FhirTools.SearchQuery.SearchQuerySupport.GetPrefixListForSearchType(SearchQueryDateTime.SearchParamTypeId);
            if (ArrayOfSupportedPrefixes.Contains(DateTimeValue.Prefix.Value))
            {
              switch (DateTimeValue.Prefix.Value)
              {
                case SearchComparator.Eq:
                  IndexDateTimePredicate = IndexDateTimePredicate.And(EqualTo(DateTimeValue.Value.Value, ISearchQueryCalcHighDateTime.SearchQueryCalculateHighDateTimeForRange(DateTimeValue.Value.Value, DateTimeValue.Precision.Value)));
                  ResultList.Add(IndexDateTimePredicate);
                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexDateTimePredicate));
                  break;
                case SearchComparator.Ne:
                  IndexDateTimePredicate = IndexDateTimePredicate.And(NotEqualTo(DateTimeValue.Value.Value, ISearchQueryCalcHighDateTime.SearchQueryCalculateHighDateTimeForRange(DateTimeValue.Value.Value, DateTimeValue.Precision.Value)));
                  ResultList.Add(IndexDateTimePredicate);

                  var SearchQueryDateTimeIdPredicate = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
                  SearchQueryDateTimeIdPredicate = SearchQueryDateTimeIdPredicate.And(IsNotSearchParameterId(SearchQueryDateTime.Id));
                  ResultList.Add(SearchQueryDateTimeIdPredicate);

                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexDateTimePredicate));
                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(SearchQueryDateTimeIdPredicate, false));
                  break;
                case SearchComparator.Gt:
                  IndexDateTimePredicate = IndexDateTimePredicate.And(GreaterThan(DateTimeValue.Value.Value, ISearchQueryCalcHighDateTime.SearchQueryCalculateHighDateTimeForRange(DateTimeValue.Value.Value, DateTimeValue.Precision.Value)));
                  ResultList.Add(IndexDateTimePredicate);
                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexDateTimePredicate));
                  break;
                case SearchComparator.Lt:
                  IndexDateTimePredicate = IndexDateTimePredicate.And(LessThan(DateTimeValue.Value.Value, ISearchQueryCalcHighDateTime.SearchQueryCalculateHighDateTimeForRange(DateTimeValue.Value.Value, DateTimeValue.Precision.Value)));
                  ResultList.Add(IndexDateTimePredicate);
                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexDateTimePredicate));
                  break;
                case SearchComparator.Ge:
                  IndexDateTimePredicate = IndexDateTimePredicate.And(GreaterThanOrEqualTo(DateTimeValue.Value.Value, ISearchQueryCalcHighDateTime.SearchQueryCalculateHighDateTimeForRange(DateTimeValue.Value.Value, DateTimeValue.Precision.Value)));
                  ResultList.Add(IndexDateTimePredicate);
                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexDateTimePredicate));
                  break;
                case SearchComparator.Le:
                  IndexDateTimePredicate = IndexDateTimePredicate.And(LessThanOrEqualTo(DateTimeValue.Value.Value, ISearchQueryCalcHighDateTime.SearchQueryCalculateHighDateTimeForRange(DateTimeValue.Value.Value, DateTimeValue.Precision.Value)));
                  ResultList.Add(IndexDateTimePredicate);
                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndex(IndexDateTimePredicate));
                  break;
                default:
                  throw new System.ComponentModel.InvalidEnumArgumentException(DateTimeValue.Prefix.Value.GetCode(), (int)DateTimeValue.Prefix.Value, typeof(SearchComparator));
              }
            }
            else
            {
              string SupportedPrefixes = String.Join(',', ArrayOfSupportedPrefixes);
              throw new ApplicationException($"Internal Server Error: The search query prefix: {DateTimeValue.Prefix.Value.GetCode()} is not supported for search parameter types of: {SearchQueryDateTime.SearchParamTypeId.GetCode()}. The supported prefixes are: {SupportedPrefixes}");
            }
          }
        }
        else
        {
          var ArrayOfSupportedModifiers = Common.FhirTools.SearchQuery.SearchQuerySupport.GetModifiersForSearchType(SearchQueryDateTime.SearchParamTypeId);
          if (ArrayOfSupportedModifiers.Contains(SearchQueryDateTime.Modifier.Value))
          {
            if (SearchQueryDateTime.Modifier.Value != SearchModifierCode.Missing)
            {
              if (DateTimeValue.Value is null)
              {
                throw new ArgumentNullException(nameof(DateTimeValue.Value));
              }
            }
            switch (SearchQueryDateTime.Modifier.Value)
            {
              case SearchModifierCode.Missing:
                if (DateTimeValue.Prefix.HasValue == false)
                {
                  IndexDateTimePredicate = IndexDateTimePredicate.And(IsNotSearchParameterId(SearchQueryDateTime.Id));
                  ResultList.Add(IndexDateTimePredicate);
                  //ResourceStorePredicate = ResourceStorePredicate.Or(AnyIndexEquals(IndexDateTimePredicate, !DateTimeValue.IsMissing));
                }
                else
                {
                  throw new ApplicationException($"Internal Server Error: Encountered a DateTime Query with a: {SearchModifierCode.Missing.GetCode()} modifier and a prefix of {DateTimeValue.Prefix!.GetCode()}. This should not happen as a missing parameter value must be only True or False with no prefix.");
                }
                break;
              default:
                throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryDateTime.Modifier.Value.GetCode()} has been added to the supported list for {SearchQueryDateTime.SearchParamTypeId.GetCode()} search parameter queries and yet no database predicate has been provided.");
            }
          }
          else
          {
            throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryDateTime.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryDateTime.SearchParamTypeId.GetCode()}.");
          }
        }
      }
      return ResultList;
    }
    private Expression<Func<ResourceStore, bool>> AnyIndex(Expression<Func<IndexDateTime, bool>> Predicate)
    {
      return x => x.DateTimeIndexList.Any(Predicate.Compile());
    }
    private Expression<Func<ResourceStore, bool>> AnyIndexEquals(Expression<Func<IndexDateTime, bool>> Predicate, bool Equals)
    {
      return x => x.DateTimeIndexList.Any(Predicate.Compile()) == Equals;
    }
    private Expression<Func<IndexDateTime, bool>> IsSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId == searchParameterId;
    }
    private Expression<Func<IndexDateTime, bool>> IsNotSearchParameterId(int searchParameterId)
    {
      return x => x.SearchParameterId != searchParameterId;
    }

    private Expression<Func<IndexDateTime, bool>> EqualTo(DateTime LowValue, DateTime HighValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexDateTime>(true);

      //PredicateOne: x => x.High >= lowValue && x.High <= highValue       
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateOne = PredicateOne.And(IndexDateTime_High_IsHigherThanOrEqualTo(LowValue));
      PredicateOne = PredicateOne.And(IndexDateTime_High_IsLowerThanOrEqualTo(HighValue));

      //PredicateTwo: x => x.High >= HighValue &&  x.Low <= LowValue
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateTwo = PredicateTwo.And(IndexDateTime_High_IsHigherThanOrEqualTo(HighValue));
      PredicateTwo = PredicateTwo.And(IndexDateTime_Low_IsLowerThanOrEqualTo(LowValue));

      //PredicateThree: x => x.Low >= LowValue && x.Low >= HighValue 
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateThree = PredicateThree.And(IndexDateTime_Low_IsHigherThanOrEqualTo(LowValue));
      PredicateThree = PredicateThree.And(IndexDateTime_Low_IsLowerThanOrEqualTo(HighValue));

      //PredicateFour: x => x.Low >= LowValue && x.High <= HighValue
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateFour = PredicateFour.And(IndexDateTime_Low_IsHigherThanOrEqualTo(LowValue));
      PredicateFour = PredicateFour.And(IndexDateTime_High_IsLowerThanOrEqualTo(HighValue));

      //PredicateFive: x => x.High == null < midValue && x.Low <= HighValue
      var PredicateFive = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateFive = PredicateFive.And(IndexDateTime_High_IsNull());
      PredicateFive = PredicateFive.And(IndexDateTime_Low_IsLowerThanOrEqualTo(HighValue));

      //PredicateSix: x => x.Low == null && x.High >= HighValue
      var PredicateSix = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateSix = PredicateSix.And(IndexDateTime_Low_IsNull());
      PredicateSix = PredicateSix.And(IndexDateTime_High_IsHigherThanOrEqualTo(HighValue));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);
      PredicateMain = PredicateMain.Or(PredicateFive);
      PredicateMain = PredicateMain.Or(PredicateSix);
      
      return PredicateMain;
    }
    private Expression<Func<IndexDateTime, bool>> NotEqualTo(DateTime LowValue, DateTime HighValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexDateTime>(true);

      //PredicateOne: x => x.High == null && x.Low > HighValue
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateOne = PredicateOne.And(IndexDateTime_High_IsNull());
      PredicateOne = PredicateOne.And(IndexDateTime_Low_IsHigherThan(HighValue));

      //PredicateTwo: x => x.Low == null && x.High < LowValue  
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateTwo = PredicateTwo.And(IndexDateTime_Low_IsNull());
      PredicateTwo = PredicateTwo.And(IndexDateTime_High_IsLowerThen(LowValue));

      //PredicateThree: x => x.High > LowValue && x.High < HighValue  
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateThree = PredicateThree.And(IndexDateTime_High_IsLowerThen(LowValue));
      PredicateThree = PredicateThree.And(IndexDateTime_High_IsLowerThen(HighValue));

      //PredicateThree: x => x.Low > HighValue && x.Low > LowValue
      var PredicateFour = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateFour = PredicateFour.And(IndexDateTime_Low_IsHigherThan(HighValue));
      PredicateFour = PredicateFour.And(IndexDateTime_Low_IsHigherThan(LowValue));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);
      PredicateMain = PredicateMain.Or(PredicateFour);


      return PredicateMain;
    }
    private Expression<Func<IndexDateTime, bool>> GreaterThan(DateTime LowValue, DateTime HighValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexDateTime>(true);

      //PredicateOne: x => x.Low == null && x.High > HighValue 
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateOne = PredicateOne.And(IndexDateTime_Low_IsNull());
      PredicateOne = PredicateOne.And(IndexDateTime_High_IsHigherThan(HighValue));

      //PredicateTwo: x => x.High == null && x.Low != null
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateTwo = PredicateTwo.And(IndexDateTime_High_IsNull());
      PredicateTwo = PredicateTwo.And(IndexDateTime_Low_IsNotNull());

      //PredicateThree: x => x.High > HighValue
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateThree = PredicateThree.And(IndexDateTime_High_IsHigherThan(HighValue));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);

      return PredicateMain;
    }
    private Expression<Func<IndexDateTime, bool>> GreaterThanOrEqualTo(DateTime LowValue, DateTime HighValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexDateTime>(true);

      //PredicateOne: x => x.Low == null && x.High >= LowValue      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateOne = PredicateOne.And(IndexDateTime_Low_IsNull());
      PredicateOne = PredicateOne.And(IndexDateTime_High_IsHigherThanOrEqualTo(LowValue));

      //PredicateTwo: x => x.Low =! null && x.High >= LowValue 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateTwo = PredicateTwo.Or(IndexDateTime_Low_IsNotNull());
      PredicateTwo = PredicateTwo.Or(IndexDateTime_High_IsHigherThanOrEqualTo(LowValue));

      //PredicateThree: x => x.Low != null && x.High == null 
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateThree = PredicateThree.And(IndexDateTime_Low_IsNotNull());
      PredicateThree = PredicateThree.And(IndexDateTime_High_IsNull());

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);

      return PredicateMain;
    }
    private Expression<Func<IndexDateTime, bool>> LessThan(DateTime LowValue, DateTime HighValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexDateTime>(true);

      //PredicateOne: x => x.Low == null && x.High < LowValue      
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateOne = PredicateOne.And(IndexDateTime_Low_IsNull());
      PredicateOne = PredicateOne.And(IndexDateTime_High_IsLowerThen(LowValue));

      //PredicateTwo: x => x.Low =! null && x.High < LowValue 
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateTwo = PredicateTwo.And(IndexDateTime_Low_IsNotNull());
      PredicateTwo = PredicateTwo.And(IndexDateTime_High_IsLowerThen(LowValue));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);

      return PredicateMain;
    }
    private Expression<Func<IndexDateTime, bool>> LessThanOrEqualTo(DateTime LowValue, DateTime HighValue)
    {
      var PredicateMain = LinqKit.PredicateBuilder.New<IndexDateTime>(true);

      //PredicateOne: x => x.Low == null && x.High <= HighValue
      var PredicateOne = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateOne = PredicateOne.And(IndexDateTime_Low_IsNull());
      PredicateOne = PredicateOne.And(IndexDateTime_High_IsLowerThanOrEqualTo(HighValue));

      //PredicateTwo: x => x.High == null || x.Low <= HighValue
      var PredicateTwo = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateTwo = PredicateTwo.And(IndexDateTime_High_IsNull());
      PredicateTwo = PredicateTwo.And(IndexDateTime_Low_IsLowerThanOrEqualTo(HighValue));

      //PredicateThree: x => x.Low <= LowValue
      var PredicateThree = LinqKit.PredicateBuilder.New<IndexDateTime>(true);
      PredicateThree = PredicateThree.And(IndexDateTime_Low_IsLowerThanOrEqualTo(LowValue));

      PredicateMain = PredicateMain.Or(PredicateOne);
      PredicateMain = PredicateMain.Or(PredicateTwo);
      PredicateMain = PredicateMain.Or(PredicateThree);

      return PredicateMain;
    }

    //High
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_High_IsNull()
    {
      return x => x.High == null;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_High_IsHigherThan(DateTime value)
    {
      return x => x.High > value;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_High_IsHigherThanOrEqualTo(DateTime value)
    {
      return x => x.High >= value;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_High_IsLowerThen(DateTime value)
    {
      return x => x.High < value;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_High_IsLowerThanOrEqualTo(DateTime value)
    {
      return x => x.High <= value;
    }

    //Low
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_Low_IsNull()
    {
      return x => x.Low == null;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_Low_IsNotNull()
    {
      return x => x.Low != null;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_Low_IsHigherThan(DateTime value)
    {
      return x => x.Low > value;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_Low_IsHigherThanOrEqualTo(DateTime value)
    {
      return x => x.Low >= value;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_Low_IsLowerThan(DateTime value)
    {
      return x => x.Low < value;
    }
    private Expression<Func<IndexDateTime, bool>> IndexDateTime_Low_IsLowerThanOrEqualTo(DateTime value)
    {
      return x => x.Low <= value;
    }

  }
}
