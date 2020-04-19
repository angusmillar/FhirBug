using Bug.Common.Enums;
using System;

namespace Bug.Common.DateTimeTools
{
  public interface ISearchQueryCalcHighDateTime
  {
    DateTime SearchQueryCalculateHighDateTimeForRange(DateTime LowValue, DateTimePrecision Precision);
  }
}