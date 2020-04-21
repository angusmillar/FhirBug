using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Hl7.Fhir.Model.Primitives;
using System;

namespace Bug.Common.DateTimeTools
{
  public class FhirDateTimeFactory : IFhirDateTimeFactory
  {
    private readonly IServerDefaultTimeZoneTimeSpan IServerDefaultTimeZoneTimeSpan;
    private readonly char MinusTimeZoneDelimiter = '-';
    private readonly char PlusTimeZoneDelimiter = '+';
    private readonly string TimeDelimiter = "T";
    private readonly static string MilliSecDelimiter = ".";
    private readonly string HourMinSecDelimiter = ":";
    private readonly char TermZulu = 'Z';
    private readonly string[] AllowedFormats = new string[] {
       //Without TimeZone Info       
       //The ten millionths of a second in a date and time value.
       "yyyy-MM-ddTHH:mm:ss.fffffff"
        //The millionths of a second in a date and time value.
      ,"yyyy-MM-ddTHH:mm:ss.ffffff"
        //The hundred thousandths of a second in a date and time value.
      ,"yyyy-MM-ddTHH:mm:ss.fffff"
        //The ten thousandths of a second in a date and time value.
      ,"yyyy-MM-ddTHH:mm:ss.ffff"
        //The milliseconds in a date and time value.
      ,"yyyy-MM-ddTHH:mm:ss.fff"
        //The hundredths of a second in a date and time value.
      ,"yyyy-MM-ddTHH:mm:ss.ff"
        //The tenths of a second in a date and time value.
      ,"yyyy-MM-ddTHH:mm:ss.f"
      ,"yyyy-MM-ddTHH:mm:ss"
      ,"yyyy-MM-ddTHH:mm"
      ,"yyyy-MM-dd"
      ,"yyyy-MM"
      ,"yyyy"
      //With numeric TimeZone Info e.g '+08:00' or '-08:00'      
      ,"yyyy-MM-ddTHH:mm:ss.fffffffzzz"
      ,"yyyy-MM-ddTHH:mm:ss.ffffffzzz"
      ,"yyyy-MM-ddTHH:mm:ss.fffffzzz"
      ,"yyyy-MM-ddTHH:mm:ss.ffffzzz"
      ,"yyyy-MM-ddTHH:mm:ss.fffzzz"
      ,"yyyy-MM-ddTHH:mm:ss.ffzzz"
      ,"yyyy-MM-ddTHH:mm:ss.fzzz"
      ,"yyyy-MM-ddTHH:mm:sszzz"
      ,"yyyy-MM-ddTHH:mmzzz"
      //With Zulu TimeZone e.g 'Z'      
      ,"yyyy-MM-ddTHH:mm:ss.fffffffK"
      ,"yyyy-MM-ddTHH:mm:ss.ffffffK"
      ,"yyyy-MM-ddTHH:mm:ss.fffffK"
      ,"yyyy-MM-ddTHH:mm:ss.ffffK"
      ,"yyyy-MM-ddTHH:mm:ss.fffK"
      ,"yyyy-MM-ddTHH:mm:ss.ffK"
      ,"yyyy-MM-ddTHH:mm:ss.fK"
      ,"yyyy-MM-ddTHH:mm:ssK"
      ,"yyyy-MM-ddTHH:mmK"
    };

    public FhirDateTimeFactory(IServerDefaultTimeZoneTimeSpan IServerDefaultTimeZoneTimeSpan)
    {
      this.IServerDefaultTimeZoneTimeSpan = IServerDefaultTimeZoneTimeSpan;
    }
    public bool TryParse(string fhirDateTimeString, out FhirDateTime? fhirDateTime, out string? errorMessage)
    {
      if (string.IsNullOrWhiteSpace(fhirDateTimeString))
        throw new System.ArgumentNullException($"{nameof(fhirDateTimeString)} cannot be null of an empty string.");

      string fhirDateTimeStringWithSec = CorrectByAddingSecondsToHourMinDateTimeWithNoSeconds(fhirDateTimeString);
      //Remember that timezone's must have colons i.e +08:00 not +0800
      if (!Hl7.Fhir.Model.Primitives.PartialDateTime.TryParse(fhirDateTimeStringWithSec, out Hl7.Fhir.Model.Primitives.PartialDateTime PartialDateTime))
      {
        fhirDateTime = null;
        errorMessage = $"The FHIR dateTime string of {fhirDateTimeStringWithSec} was no able to be parsed as a FHIR DateTime datatype.";
        return false;
      }
      //I intentionally provide the fhirDateTimeString and not fhirDateTimeStringWithSec because we want the precision that was provided not the 
      //precision once the seconds have been added, so we want to know it is HourMin precision not always Sec precision.
      if (TryParsePrecision(fhirDateTimeString, out DateTimePrecision? DateTimePrecision, out bool? HasTimeZoneInfo))
      {
        if (TryParseDateTimeToUniversalTime(fhirDateTimeStringWithSec, HasTimeZoneInfo!.Value, out DateTime? DateTime, out string? ErrorMessage))
        {
          fhirDateTime = new FhirDateTime(TruncateToThounsdandsMilliseconds(DateTime!.Value), DateTimePrecision!.Value);
          errorMessage = null;
          return true;
        }
        else
        {
          fhirDateTime = null;
          errorMessage = ErrorMessage;
          return false;
        }
      }
      else
      {
        fhirDateTime = null;
        errorMessage = $"Unable to parse DateTime using FHIR format rules. Value was: {fhirDateTimeString}, no format could be determined.";
        return false;
      }
    }   

    private DateTime TruncateToThounsdandsMilliseconds(DateTime value)
    {
      return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
    }

    public bool TryParse(PartialDateTime partialDateTime, out FhirDateTime? fhirDateTime, out string? errorMessage)
    {
      if (TryParsePrecision(partialDateTime.ToString(), out DateTimePrecision? DateTimePrecision, out _))
      {
        fhirDateTime = new FhirDateTime(TruncateToThounsdandsMilliseconds(partialDateTime.ToUniversalTime().DateTime), DateTimePrecision!.Value);
        errorMessage = null;
        return true;
      }
      else
      {
        throw new ApplicationException($"Internal Server Error: Unable to determine the DateTimePrecision of a DateTime provided as a FHIR {typeof(PartialDateTime).Name} type. The string value was: {partialDateTime.ToString()}.");
      }
    }
    private bool TryParsePrecision(string value, out DateTimePrecision? dateTimePrecision, out bool? hasTimeZoneInfo)
    {
      int Legnth = value.Length;
      if (value.Split(TimeDelimiter).Length == 1)
      {
        //Must be one of (yyyy-MM-dd, yyyy-MM, yyyy)
        hasTimeZoneInfo = false;
        string[] HourMinSecDelimiterSplit = value.Split(MinusTimeZoneDelimiter);
        if (HourMinSecDelimiterSplit.Length == 1)
        {
          //2020          
          //format = "yyyy";
          dateTimePrecision = DateTimePrecision.Year;
          return true;
        }
        else if (HourMinSecDelimiterSplit.Length == 2)
        {
          //2020-04          
          //format = "yyyy-MM";
          dateTimePrecision = DateTimePrecision.Month;
          return true;

        }
        else if (HourMinSecDelimiterSplit.Length == 3)
        {
          //2020-04-19             
          //format = "yyyy-MM-dd";
          dateTimePrecision = DateTimePrecision.Day;
          return true;
        }
        else
        {
          //format = null;
          dateTimePrecision = null;
          return false;
        }
      }
      else if (value.Split(TimeDelimiter).Length == 2)
      {
        //We have time such as (yyyy-MM-ddTHH:mm, yyyy-MM-ddTHH:mm:ss.ffffffffzzz, yyyy-MM-ddTHH:mm:ss.ffK or many others)
        if (value.EndsWith(TermZulu))
        {
          //We have a Zulu time such as yyyy-MM-ddTHH:mm:ssK
          hasTimeZoneInfo = true;
          if (Legnth == 28)
          {
            //2020-04-19T10:30:25.1234567Z            
            //format = "yyyy-MM-ddTHH:mm:ss.fffffffK";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 27)
          {
            //2020-04-19T10:30:25.123456Z                        
            //format = "yyyy-MM-ddTHH:mm:ss.ffffffK";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 26)
          {
            //2020-04-19T10:30:25.12345Z                        
            //format = "yyyy-MM-ddTHH:mm:ss.fffffK";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 25)
          {
            //2020-04-19T10:30:25.1234Z                                    
            //format = "yyyy-MM-ddTHH:mm:ss.ffffK";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 24)
          {
            //2020-04-19T10:30:25.123Z                                                
            //format = "yyyy-MM-ddTHH:mm:ss.fffK";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 23)
          {
            //2020-04-19T10:30:25.12Z                                                            
            //format = "yyyy-MM-ddTHH:mm:ss.ffK";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 22)
          {
            //2020-04-19T10:30:25.1Z             
            //format = "yyyy-MM-ddTHH:mm:ss.fK";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 20)
          {
            //2020-04-19T10:30:25Z                         
            //format = "yyyy-MM-ddTHH:mm:ssK";
            dateTimePrecision = DateTimePrecision.Sec;
            return true;
          }
          else if (Legnth == 17)
          {
            //2020-04-19T10:30Z                                     
            //format = "yyyy-MM-ddTHH:mmK";
            dateTimePrecision = DateTimePrecision.HourMin;
            return true;
          }
          else
          {
            //format = null;
            dateTimePrecision = null;
            hasTimeZoneInfo = null;
            return false;
          }
        }
        else if (value.Split(TimeDelimiter)[1].Contains(MinusTimeZoneDelimiter) || value.Split(TimeDelimiter)[1].Contains(PlusTimeZoneDelimiter))
        {
          //We have a numeric timezone on the end such as +08:00 or -08:00
          hasTimeZoneInfo = true;
          if (Legnth == 33)
          {
            //2020-04-19T10:30:25.1234567+08:00                                    
            //format = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 32)
          {
            //2020-04-19T10:30:25.123456+08:00                                                
            //format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 31)
          {
            //2020-04-19T10:30:25.12345+08:00                                                            
            //format = "yyyy-MM-ddTHH:mm:ss.fffffzzz";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 30)
          {
            //2020-04-19T10:30:25.1234+08:00                     
            //format = "yyyy-MM-ddTHH:mm:ss.ffffzzz";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 29)
          {
            //2020-04-19T10:30:25.123+08:00              
            //format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 28)
          {
            //2020-04-19T10:30:25.12+08:00                          
            //format = "yyyy-MM-ddTHH:mm:ss.ffzzz";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 27)
          {
            //2020-04-19T10:30:25.1+08:00            
            //format = "yyyy-MM-ddTHH:mm:ss.fzzz";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 25)
          {
            //2020-04-19T10:30:25+08:00                        
            //format = "yyyy-MM-ddTHH:mm:sszzz";
            dateTimePrecision = DateTimePrecision.Sec;
            return true;
          }
          else if (Legnth == 22)
          {
            //2020-04-19T10:30+08:00                                    
            //format = "yyyy-MM-ddTHH:mmzzz";
            dateTimePrecision = DateTimePrecision.HourMin;
            return true;
          }
          else
          {
            //format = null;
            dateTimePrecision = null;
            hasTimeZoneInfo = null;
            return false;
          }
        }
        else
        {
          //We have no timezone info
          hasTimeZoneInfo = false;
          if (Legnth == 27)
          {
            //2020-04-19T10:30:25.1234567                          
            //format = "yyyy-MM-ddTHH:mm:ss.fffffff";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 26)
          {
            //2020-04-19T10:30:25.123456                                      
            //format = "yyyy-MM-ddTHH:mm:ss.ffffff";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 25)
          {
            //2020-04-19T10:30:25.12345            
            //format = "yyyy-MM-ddTHH:mm:ss.fffff";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 24)
          {
            //2020-04-19T10:30:25.1234            
            //format = "yyyy-MM-ddTHH:mm:ss.ffff";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 23)
          {
            //2020-04-19T10:30:25.123             
            //format = "yyyy-MM-ddTHH:mm:ss.fff";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 22)
          {
            //2020-04-19T10:30:25.12             
            //format = "yyyy-MM-ddTHH:mm:ss.ff";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 21)
          {
            //2020-04-19T10:30:25.1             
            //format = "yyyy-MM-ddTHH:mm:ss.f";
            dateTimePrecision = DateTimePrecision.MilliSec;
            return true;
          }
          else if (Legnth == 19)
          {
            //2020-04-19T10:30:25            
            //format = "yyyy-MM-ddTHH:mm:ss";
            dateTimePrecision = DateTimePrecision.Sec;
            return true;
          }
          else if (Legnth == 16)
          {
            //2020-04-19T10:30                        
            //format = "yyyy-MM-ddTHH:mm";
            dateTimePrecision = DateTimePrecision.HourMin;
            return true;
          }
          else
          {
            //format = null;
            dateTimePrecision = null;
            hasTimeZoneInfo = null;
            return false;
          }
        }
      }
      else
      {
        //format = null;
        dateTimePrecision = null;
        hasTimeZoneInfo = null;
        return false;
      }
    }

    private string CorrectByAddingSecondsToHourMinDateTimeWithNoSeconds(string FhirDateTime)
    {
      string SecondsToAdd = "00";
      //Correct dateTimes that have no seconds yet do have Hours and Min
      //2017-04-28T18:29:15+10:00   
      //2017-04-28T18:29+10:00      
      //2017-04-28T18:29Z
      //2017-04-28T18:29

      if (FhirDateTime.Length <= 10)
      {
        //it is only a date 2020 or 2020-04 or 2020-04-28
        return FhirDateTime;
      }

      if (FhirDateTime.Contains(MilliSecDelimiter))
      {
        //If it has a '.' then it has to have seconds or the format is incorrect and that will be picked up later
        return FhirDateTime;
      }

      if (FhirDateTime.Split(HourMinSecDelimiter).Length == 4)
      {
        //2017-04-28T18:29:15+10:00
        //If we split on ':' and have 4 parts then we must have seconds or we have an incorrectly formated date that will be picked up later 
        return FhirDateTime;
      }

      if (FhirDateTime.Contains(TimeDelimiter) && FhirDateTime.Split(TimeDelimiter)[1].Contains(MinusTimeZoneDelimiter) && FhirDateTime.Split(HourMinSecDelimiter).Length < 4 )
      {        
        //2017-04-28T18:29-10:00
        var SplitMinus = FhirDateTime.Split(MinusTimeZoneDelimiter);
        return $"{SplitMinus[0]}{HourMinSecDelimiter}{SecondsToAdd}{MinusTimeZoneDelimiter}{SplitMinus[1]}";
      }
      else if (FhirDateTime.Contains(TimeDelimiter) && FhirDateTime.Split(TimeDelimiter)[1].Contains(PlusTimeZoneDelimiter) && FhirDateTime.Split(HourMinSecDelimiter).Length < 4)
      {
        //2017-04-28T18:29+10:00   
        var SplitPlus = FhirDateTime.Split(PlusTimeZoneDelimiter);
        return $"{SplitPlus[0]}{HourMinSecDelimiter}{SecondsToAdd}{PlusTimeZoneDelimiter}{SplitPlus[1]}";
      }
      else if (FhirDateTime.Contains(TermZulu) && FhirDateTime.Split(HourMinSecDelimiter).Length < 3)
      {        
        //2017-04-28T18:29Z
        var SplitZulu = FhirDateTime.Split(TermZulu);
        return $"{SplitZulu[0]}{HourMinSecDelimiter}{SecondsToAdd}{TermZulu}";
      }
      else if (FhirDateTime.Split(HourMinSecDelimiter).Length < 3)
      {
        //2017-04-28T18:29
        return $"{FhirDateTime}{HourMinSecDelimiter}{SecondsToAdd}";
      }
      else
      {
        return FhirDateTime;
      }

    }

    private string CorrectByAddingSecondsToHourMinDateTimeWithNoSecondsOLD(string FhirDateTime)
    {
      //Correct dateTimes that have no seconds yet do have Hours and Min
      //2017-04-28T18:29+10:00      
      //2017-04-28T18:29Z
      //2017-04-28T18:29

      //2020-04-19T10:30:25.12345+08:00 

      string SecondsToAdd = "00";
      var Split = FhirDateTime.Split(HourMinSecDelimiter.ToCharArray());
      string New = string.Empty;
      string Temp = string.Empty;
      if ((FhirDateTime.Length == 22 && FhirDateTime.Substring(19, 1) == HourMinSecDelimiter) || (FhirDateTime.Length == 16))
      {
        if (FhirDateTime.Length > 16)
        {
          //"yyyy-MM-ddTHH:mm" convert to "yyyy-MM-ddTHH:mm:ss"
          //Value has a timezone
          if (Split[1].Contains(MinusTimeZoneDelimiter))
          {
            Temp = $"{Split[1].Split(MinusTimeZoneDelimiter)[0]}{HourMinSecDelimiter}{SecondsToAdd}{MinusTimeZoneDelimiter}{Split[1].Split(MinusTimeZoneDelimiter)[1]}";
          }
          else if (Split[1].Contains(PlusTimeZoneDelimiter))
          {
            Temp = $"{Split[1].Split(PlusTimeZoneDelimiter)[0]}{HourMinSecDelimiter}{SecondsToAdd}{PlusTimeZoneDelimiter}{Split[1].Split(PlusTimeZoneDelimiter)[1]}";
          }
          return $"{Split[0]}{HourMinSecDelimiter}{Temp}{HourMinSecDelimiter}{Split[2]}";
        }
        else
        {
          //"yyyy-MM-ddTHH:mmzzz" convert to "yyyy-MM-ddTHH:mm:sszzz"
          return $"{Split[0]}{HourMinSecDelimiter}{Split[1]}{HourMinSecDelimiter}{SecondsToAdd}";
        }
      }
      else
      {
        return FhirDateTime;
      }
    }
    private bool TryParseDateTimeToUniversalTime(string value, bool HasTimeZone, out DateTime? dateTime, out string? errorMessage)
    {
      if (HasTimeZone)
      {
        //As we have timezone info in the string we can parse straight to DateTimeOffset and then to UniversalTime
        if (DateTimeOffset.TryParseExact(value, this.AllowedFormats, null, System.Globalization.DateTimeStyles.None, out DateTimeOffset DateTimeOffsetFinal))
        {
          dateTime = DateTimeOffsetFinal.ToZulu();
          errorMessage = null;
          return true;
        }
        else
        {
          dateTime = null;
          errorMessage = $"Error parsing a FHIR DateTime with timezone info. Value was: {value}.";
          return false;
        }
      }
      else
      {
        //As we have no timezone info we must first parse to DateTime, then set as local timezone before converting to UniversalTime
        if (DateTime.TryParseExact(value, this.AllowedFormats, null, System.Globalization.DateTimeStyles.None, out DateTime DateTimeOut))
        {
          DateTimeOffset DateTimeOffsetFinal = new DateTimeOffset(DateTimeOut, IServerDefaultTimeZoneTimeSpan.ServerDefaultTimeZoneTimeSpan);
          dateTime = DateTimeOffsetFinal.ToZulu();
          errorMessage = null;
          return true;
        }
        else
        {
          dateTime = null;
          errorMessage = $"Error parsing a FHIR DateTime with no time zone info. Value was {value}.";
          return false;
        }
      }
    }
  }
}
