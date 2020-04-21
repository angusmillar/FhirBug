using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Common.DateTimeTools
{
  public class FhirDateTimeSupport : IIndexSettingCalcHighDateTime, ISearchQueryCalcHighDateTime
  {
    public DateTime SearchQueryCalculateHighDateTimeForRange(DateTime LowValue, DateTimePrecision Precision)
    {
      DateTime HighDateTime = LowValue;
      if (Precision == DateTimePrecision.Year)
      {
        //To deal with the problem of no time zones on Dates, e.g 2018-10-05 we treat the search as a 36 hour day rather than a 24 hours day
        //When the precision is one on Year, Month or Day. For more find grained precisions such as Hour, Min, Sec we  expected to have the 
        //time zones information surplus, if time zone differences are a concern of the searcher.
        //
        //So to do this I subtract 6 hours from the beginning of the date range 2018-10-05T00:00 and we add 6 hours to the end of the day 2018-10-05T23:59
        //This gives us a 36 hour day range. The idea is that it is better to return more than less for the search.
        //This is a compromise as we really do not know what is meant by a date with no time zone. We can assume the servers default time zone as a starting point
        //but this is only a guess to what the true time zone was for either the supplied search date or the stored FHIR resource dates, when dealing with only date 
        //and no time.  
        //
        //So the range we actually use for this example is not:   
        //  2018-10-05T00:00 to 2018-10-05T23:59 
        //but rather: 
        //  2018-10-04T18:00 to 2018-10-06T05:59 
        //which in a 12hr clock is 04/10/2018 6:00PM to 06/10/2018 6:00AM when the search date was: 05/10/2018
        //Also bare in mind that all date times are converted to UTC Zulu +00:00 time when stored and searched in the database.

        //Work out the normal 24 hour day range low and high
        HighDateTime = LowValue.AddYears(1).AddMilliseconds(-1);

        //Subtract 6 hours from the low
        LowValue = LowValue.AddHours(-6);
        //Add 6 hours to the high
        HighDateTime = HighDateTime.AddHours(6);

      }
      else if (Precision == DateTimePrecision.Month)
      {
        //Work out the normal 24 hour day range low and high
        HighDateTime = LowValue.AddMonths(1).AddMilliseconds(-1);

        //Subtract 6 hours from the low
        LowValue = LowValue.AddHours(-6);
        //Add 6 hours to the high
        HighDateTime = HighDateTime.AddHours(6);
      }
      else if (Precision == DateTimePrecision.Day)
      {
        //Work out the normal 24 hour day range low and high
        HighDateTime = LowValue.AddDays(1).AddMilliseconds(-1);

        //Subtract 6 hours from the low
        LowValue = LowValue.AddHours(-6);
        //Add 6 hours to the high
        HighDateTime = HighDateTime.AddHours(6);

      }
      else if (Precision == DateTimePrecision.HourMin)
      {
        HighDateTime = LowValue.AddMinutes(1).AddMilliseconds(-1);
      }
      else if (Precision == DateTimePrecision.Sec)
      {
        HighDateTime = LowValue.AddSeconds(1).AddMilliseconds(-1);
      }
      else if (Precision == DateTimePrecision.MilliSec)
      {
        HighDateTime = LowValue.AddMilliseconds(1).AddTicks(-999);
      }
      else
      {
        throw new System.ComponentModel.InvalidEnumArgumentException(Precision.ToString(), (int)Precision, typeof(DateTimePrecision));
      }
      return HighDateTime;
    }

    public DateTime IndexSettingCalculateHighDateTimeForRangeOLD(DateTime LowValue, DateTimePrecision Precision)
    {
      switch (Precision)
      {
        case Bug.Common.Enums.DateTimePrecision.Year:
          return LowValue.AddYears(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.Month:
          return LowValue.AddMonths(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.Day:
          return LowValue.AddDays(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.HourMin:
          return LowValue.AddSeconds(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.Sec:
          return LowValue.AddMilliseconds(999);
        case Bug.Common.Enums.DateTimePrecision.MilliSec:
          return LowValue.AddMilliseconds(1).AddTicks(-1);
        default:
          throw new System.ComponentModel.InvalidEnumArgumentException(Precision.GetCode(), (int)Precision, typeof(DateTimePrecision));
      }
    }

    public DateTime IndexSettingCalculateHighDateTimeForRange(DateTime LowValue, DateTimePrecision Precision)
    {
      switch (Precision)
      {
        case Bug.Common.Enums.DateTimePrecision.Year:
          return LowValue.AddYears(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.Month:
          return LowValue.AddMonths(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.Day:
          return LowValue.AddDays(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.HourMin:
          return LowValue.AddMinutes(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.Sec:
          return LowValue.AddSeconds(1).AddMilliseconds(-1);
        case Bug.Common.Enums.DateTimePrecision.MilliSec:
          return LowValue.AddMilliseconds(1).AddTicks(-999);
        default:
          throw new System.ComponentModel.InvalidEnumArgumentException(Precision.GetCode(), (int)Precision, typeof(DateTimePrecision));
      }
    }

  }
}
