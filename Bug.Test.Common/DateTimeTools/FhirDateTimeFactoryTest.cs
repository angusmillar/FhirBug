using Bug.Common.DateTimeTools;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;

namespace Bug.Test.Common.DateTimeTools
{
  public class FhirDateTimeFactoryTest
  {
    //Remember that this test's timezone Timespan dependent on the default timezone for the server which was set at +10:00 
    //This is found in the AppSettings file in the property named:  'ServerDefaultTimeZoneTimeSpan'
    [Theory]    
    //Numeric timezone info
    [InlineData("2020-04-18T20:46:02.1234567+10:00", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.123456+10:00", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.12345+10:00", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.1234+10:00", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.123+10:00", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.12+10:00", 2020, 04, 18, 20, 46, 02, 120, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.1+10:00", 2020, 04, 18, 20, 46, 02, 100, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02+10:00", 2020, 04, 18, 20, 46, 02, 000, 10, DateTimePrecision.Sec)]
    [InlineData("2020-04-18T20:46+10:00", 2020, 04, 18, 20, 46, 00, 000, 10, DateTimePrecision.HourMin)]
    [InlineData("2020-04-18", 2020, 04, 18, 0, 0, 00, 000, 10, DateTimePrecision.Day)]
    [InlineData("2020-04", 2020, 04, 1, 00, 0, 00, 000, 10, DateTimePrecision.Month)]
    [InlineData("2020", 2020, 1, 1, 0, 0, 00, 000, 10, DateTimePrecision.Year)]
    //Zulu 'Z' timezone info
    [InlineData("2020-04-18T20:46:02.1234567Z", 2020, 04, 18, 20, 46, 02, 123, 0, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.123456Z", 2020, 04, 18, 20, 46, 02, 123, 0, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.12345Z", 2020, 04, 18, 20, 46, 02, 123, 0, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.1234Z", 2020, 04, 18, 20, 46, 02, 123, 0, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.123Z", 2020, 04, 18, 20, 46, 02, 123, 0, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.12Z", 2020, 04, 18, 20, 46, 02, 120, 0, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.1Z", 2020, 04, 18, 20, 46, 02, 100, 0, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02Z", 2020, 04, 18, 20, 46, 02, 000, 0, DateTimePrecision.Sec)]
    [InlineData("2020-04-18T20:46Z", 2020, 04, 18, 20, 46, 00, 000, 0, DateTimePrecision.HourMin)]
    //No timezone info
    [InlineData("2020-04-18T20:46:02.1234567", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.123456", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.12345", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.1234", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.123", 2020, 04, 18, 20, 46, 02, 123, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.12", 2020, 04, 18, 20, 46, 02, 120, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02.1", 2020, 04, 18, 20, 46, 02, 100, 10, DateTimePrecision.MilliSec)]
    [InlineData("2020-04-18T20:46:02", 2020, 04, 18, 20, 46, 02, 000, 10, DateTimePrecision.Sec)]
    [InlineData("2020-04-18T20:46", 2020, 04, 18, 20, 46, 00, 000, 10, DateTimePrecision.HourMin)]

    public void TestOne(string FhirDateTimeString, int ExpectedYear, int ExpectedMonth, int ExpectedDay, int ExpectedHour, int ExpectedMin, int ExpectedSec, int ExpectedMilliSec, int TimeZoneHour, DateTimePrecision Precision)
    {
      //Prepare
      TimeSpan ServersDefaultTimeZone = TimeSpan.FromHours(10);
      var FhirDateTimeFactory = new FhirDateTimeFactory(GetIServerDefaultTimeZoneTimeSpanMock(ServersDefaultTimeZone).Object);
      var ExpectedDateTimeUTC = new DateTimeOffset(ExpectedYear, ExpectedMonth, ExpectedDay, ExpectedHour, ExpectedMin, ExpectedSec, ExpectedMilliSec, TimeSpan.FromHours(TimeZoneHour)).ToUniversalTime().DateTime; 

      if (FhirDateTimeFactory.TryParse(FhirDateTimeString, out FhirDateTime? FhirDateTime, out string? ErrorMessage))
      {
        Assert.Equal(ExpectedDateTimeUTC, FhirDateTime!.DateTime);
        Assert.Equal(Precision, FhirDateTime.Precision);
      }
      else
      {
        Assert.True(false, $"Positive test must be true, we should not get this error message: {ErrorMessage}");
      }
        
    }


    public static Mock<IServerDefaultTimeZoneTimeSpan> GetIServerDefaultTimeZoneTimeSpanMock(TimeSpan ServerDefaultTimeZone)
    {
      Mock<IServerDefaultTimeZoneTimeSpan> IServerDefaultTimeZoneTimeSpanMock = new Mock<IServerDefaultTimeZoneTimeSpan>();
      IServerDefaultTimeZoneTimeSpanMock.Setup(x =>
      x.ServerDefaultTimeZoneTimeSpan).Returns(ServerDefaultTimeZone);
      return IServerDefaultTimeZoneTimeSpanMock;
    }
  }
}
