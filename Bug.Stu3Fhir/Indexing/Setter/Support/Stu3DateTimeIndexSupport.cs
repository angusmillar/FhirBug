using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Dto.Indexing;
using Hl7.Fhir.Model;
using Hl7.Fhir.Model.Primitives;

namespace Bug.Stu3Fhir.Indexing.Setter.Support
{
  public class Stu3DateTimeIndexSupport : IStu3DateTimeIndexSupport
  {
    private readonly Common.DateTimeTools.IFhirDateTimeFactory IFhirDateTimeFactory;
    private readonly Common.DateTimeTools.IIndexSettingCalcHighDateTime IIndexSettingCalcHighDateTime;
    public Stu3DateTimeIndexSupport(Common.DateTimeTools.IFhirDateTimeFactory IFhirDateTimeFactory,
      Common.DateTimeTools.IIndexSettingCalcHighDateTime IIndexSettingCalcHighDateTime)
    {
      this.IFhirDateTimeFactory = IFhirDateTimeFactory;
      this.IIndexSettingCalcHighDateTime = IIndexSettingCalcHighDateTime;
    }

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
      if (value.StartElement != null)
      {
        PartialDateTime? PartialDateTimeLow = value.StartElement.ToPartialDateTime();
        if (PartialDateTimeLow.HasValue)
          DateTimeIndexStart = ParsePartialDateTime(PartialDateTimeLow.Value, searchParameterId);
      }

      if (value.EndElement != null)
      {
        PartialDateTime? PartialDateTimeHigh = value.EndElement.ToPartialDateTime();
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
        var DateTimeIndex = new IndexDateTime(searchParameterId)
        {
          Low = ResolveTargetEventDateTime(Timing, true, searchParameterId)
        };
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
      if (IFhirDateTimeFactory.TryParse(PartialDateTimeType, out Common.DateTimeTools.FhirDateTime? BugFhirDateTime, out string? ErrorMessage))
      {
        DateTime Low = BugFhirDateTime!.DateTime;
        DateTime High = IIndexSettingCalcHighDateTime.IndexSettingCalculateHighDateTimeForRange(Low, BugFhirDateTime.Precision);
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
