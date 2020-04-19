using Bug.Common.Enums;
using System;

namespace Bug.Common.DateTimeTools
{
  public interface IIndexSettingCalcHighDateTime
  {
    DateTime IndexSettingCalculateHighDateTimeForRange(DateTime LowValue, DateTimePrecision Precision);
  }
}