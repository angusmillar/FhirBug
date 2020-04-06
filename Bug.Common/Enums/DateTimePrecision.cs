using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum DateTimePrecision 
  {
    [EnumInfo("Year", "Year")]
    Year,
    [EnumInfo("Month", "Month")]
    Month,
    [EnumInfo("Day", "Day")]
    Day,
    [EnumInfo("HourMin", "HourMin")]
    HourMin,
    [EnumInfo("Sec", "Sec")]
    Sec,
    [EnumInfo("MilliSec", "MilliSec")]
    MilliSec,
    [EnumInfo("Tick", "Tick")]
    Tick 
  };
}
