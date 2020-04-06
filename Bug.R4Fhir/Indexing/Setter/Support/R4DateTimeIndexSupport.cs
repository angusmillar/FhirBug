using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Dto.Indexing;
using Hl7.Fhir.Model;
using Hl7.Fhir.Model.Primitives;

namespace Bug.R4Fhir.Indexing.Setter.Support
{
  public class R4DateTimeIndexSupport : IR4DateTimeIndexSupport
  {    
    public IndexDateTime? GetDateTimeIndex(Date value, int searchParameterId)
    {      
      PartialDateTime? PartialDateTimeType = value.ToPartialDateTime();
      if (PartialDateTimeType.HasValue)
        return ParsePartialDateTime(PartialDateTimeType.Value, searchParameterId);
      else
        return null;
    }

    public IndexDateTime? GetDateTimeIndex(FhirDateTime value, int searchParameterId)
    {
      PartialDateTime? PartialDateTimeType = value.ToPartialDateTime();
      if (PartialDateTimeType.HasValue)
        return ParsePartialDateTime(PartialDateTimeType.Value, searchParameterId);
      else
        return null;
    }

    public IndexDateTime? GetDateTimeIndex(Instant value, int searchParameterId)
    {      
      PartialDateTime? PartialDateTimeType = value.ToPartialDateTime();
      if (PartialDateTimeType.HasValue)
        return ParsePartialDateTime(PartialDateTimeType.Value, searchParameterId);
      else
        return null;
    }

    public IndexDateTime? GetDateTimeIndex(Period value, int searchParameterId)
    {      
      IndexDateTime? DateTimeIndexStart = null;
      IndexDateTime? DateTimeIndexEnd = null;
      
      PartialDateTime? PartialDateTimeLow = null;
      if (value.StartElement != null)
      {
        PartialDateTimeLow = value.StartElement.ToPartialDateTime();
        if (PartialDateTimeLow.HasValue)
          DateTimeIndexStart = ParsePartialDateTime(PartialDateTimeLow.Value, searchParameterId);
      }

      PartialDateTime? PartialDateTimeHigh = null;
      if (value.EndElement != null)
      {
        PartialDateTimeHigh = value.EndElement.ToPartialDateTime();
        if (PartialDateTimeHigh.HasValue)
          DateTimeIndexEnd = ParsePartialDateTime(PartialDateTimeHigh.Value, searchParameterId);
      }

      var DateTimeIndex = new IndexDateTime(searchParameterId);
      if (DateTimeIndexStart is object)
      {
        DateTimeIndex.Low = DateTimeIndexStart.Low;
      }
      if (DateTimeIndexEnd is object)
      {
        DateTimeIndex.High = DateTimeIndexEnd.High;
      }
        

      return DateTimeIndex;
    }

    public IndexDateTime? GetDateTimeIndex(Timing Timing, int searchParameterId)
    {      
      if (Timing.Event != null)
      {
        var DateTimeIndex = new IndexDateTime(searchParameterId);
        DateTimeIndex.Low = ResolveTargetEventDateTime(Timing, true, searchParameterId);
        if (DateTimeIndex.Low != DateTimeOffset.MaxValue.ToUniversalTime())
        {
          decimal TargetDuration = ResolveTargetDurationValue(Timing);
          Timing.UnitsOfTime? TargetUnitsOfTime = null;
          if (TargetDuration > decimal.Zero)
          {
            if (Timing.Repeat.DurationUnit.HasValue)
              TargetUnitsOfTime = Timing.Repeat.DurationUnit.Value;
          }

          if (TargetDuration > decimal.Zero && TargetUnitsOfTime.HasValue)
          {
            DateTimeIndex.High = AddDurationTimeToEvent(ResolveTargetEventDateTime(Timing, false, searchParameterId), TargetDuration, TargetUnitsOfTime.Value);
          }
          else
          {
            DateTimeIndex.High = null;
          }
        }
        else
        {
          DateTimeIndex.Low = null;
        }
        return DateTimeIndex;
      }
      return null;
    }

    private IndexDateTime? ParsePartialDateTime(PartialDateTime PartialDateTimeType, int searchParameterId)
    {
      var FhirDateTimeSupport = new Bug.Common.DateTimeTools.FhirDateTimeSupport(PartialDateTimeType.ToString());
      if (FhirDateTimeSupport.IsValid && FhirDateTimeSupport.Value is object)
      {
        DateTime Low = FhirDateTimeSupport.Value.Value;
        DateTime High = DateTime.MaxValue;

        switch (FhirDateTimeSupport.Precision)
        {
          case Bug.Common.Enums.DateTimePrecision.Year:
            High = Low.AddYears(1).AddMilliseconds(-1);
            break;
          case Bug.Common.Enums.DateTimePrecision.Month:
            High = Low.AddMonths(1).AddMilliseconds(-1);
            break;
          case Bug.Common.Enums.DateTimePrecision.Day:
            High = Low.AddDays(1).AddMilliseconds(-1);
            break;
          case Bug.Common.Enums.DateTimePrecision.HourMin:
            High = Low.AddSeconds(1).AddMilliseconds(-1);
            break;
          case Bug.Common.Enums.DateTimePrecision.Sec:
            High = Low.AddMilliseconds(999);
            break;
          case Bug.Common.Enums.DateTimePrecision.MilliSec:
            High = Low.AddMilliseconds(1).AddTicks(-1);
            break;
          case Bug.Common.Enums.DateTimePrecision.Tick:
            High = Low.AddTicks(1);
            break;
          default:
            break;
        }

        return new IndexDateTime(searchParameterId) { Low = Low, High = High };
      }
      return null;
    }

    //Check all DateTime values in the list and find the earliest value.        
    private DateTime ResolveTargetEventDateTime(Timing Timing, bool TargetLowest, int searchParameterId)
    {
      DateTime TargetEventDateTime;
      if (TargetLowest)
        TargetEventDateTime = DateTime.MaxValue.ToUniversalTime();
      else
        TargetEventDateTime = DateTime.MinValue.ToUniversalTime();

      foreach (var EventDateTime in Timing.EventElement)
      {
        if (!string.IsNullOrWhiteSpace(EventDateTime.Value))
        {
          if (FhirDateTime.IsValidValue(EventDateTime.Value))
          {
            PartialDateTime? PartialDateTimeType = EventDateTime.ToPartialDateTime();
            if (PartialDateTimeType.HasValue)
            {
              IndexDateTime? DateTimeIndexOffSetValue = ParsePartialDateTime(PartialDateTimeType.Value, searchParameterId);
              if (DateTimeIndexOffSetValue is object)
              {
                if (TargetLowest)
                {
                  if (DateTimeIndexOffSetValue.Low.HasValue)
                  {
                    if (TargetEventDateTime > DateTimeIndexOffSetValue.Low.Value)
                    {
                      TargetEventDateTime = DateTimeIndexOffSetValue.Low.Value;
                    }
                  }
                }
                else
                {
                  if (DateTimeIndexOffSetValue.High.HasValue)
                  {
                    if (TargetEventDateTime < DateTimeIndexOffSetValue.High.Value)
                    {
                      TargetEventDateTime = DateTimeIndexOffSetValue.High.Value;
                    }
                  }
                }
              }
            }
          }
        }
      }
      return TargetEventDateTime;
    }
    private decimal ResolveTargetDurationValue(Timing Timing)
    {
      decimal TargetDuration = decimal.Zero;
      decimal DurationMax = decimal.Zero;
      decimal Duration = decimal.Zero;
      if (Timing.Repeat != null)
      {
        if (Timing.Repeat.DurationMax != null)
        {
          if (Timing.Repeat.DurationMax.HasValue)
          {
            DurationMax = Timing.Repeat.DurationMax.Value;
          }
        }
        if (DurationMax == decimal.Zero)
        {
          if (Timing.Repeat.Duration != null)
          {
            if (Timing.Repeat.Duration.HasValue)
            {
              Duration = Timing.Repeat.Duration.Value;
            }
          }
        }
        if (DurationMax > decimal.Zero)
        {
          TargetDuration = DurationMax;
        }
        else if (Duration > decimal.Zero)
        {
          TargetDuration = Duration;
        }
        return TargetDuration;
      }
      return decimal.Zero;
    }
    private DateTime AddDurationTimeToEvent(DateTime FromDateTime, decimal TargetDuration, Timing.UnitsOfTime TargetUnitsOfTime)
    {
      switch (TargetUnitsOfTime)
      {
        case Timing.UnitsOfTime.S:
          {
            return FromDateTime.AddSeconds(Convert.ToDouble(TargetDuration));
          }
        case Timing.UnitsOfTime.Min:
          {
            return FromDateTime.AddMinutes(Convert.ToDouble(TargetDuration));
          }
        case Timing.UnitsOfTime.H:
          {
            return FromDateTime.AddHours(Convert.ToDouble(TargetDuration));
          }
        case Timing.UnitsOfTime.D:
          {
            return FromDateTime.AddDays(Convert.ToDouble(TargetDuration));
          }
        case Timing.UnitsOfTime.Wk:
          {
            return FromDateTime.AddDays(Convert.ToDouble(TargetDuration * 7));
          }
        case Timing.UnitsOfTime.Mo:
          {
            return FromDateTime.AddMonths(Convert.ToInt32(TargetDuration));
          }
        case Timing.UnitsOfTime.A:
          {
            return FromDateTime.AddYears(Convert.ToInt32(TargetDuration));
          }
        default:
          {
            throw new System.ComponentModel.InvalidEnumArgumentException(TargetUnitsOfTime.ToString(), (int)TargetUnitsOfTime, typeof(Timing.UnitsOfTime));
          }
      }
    }
  }
}
