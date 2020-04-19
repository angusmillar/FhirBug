using Bug.Common.DateTimeTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class IFhirDateTimeFactory_Factory
  {
    public static IFhirDateTimeFactory Get(TimeSpan DefaultServerTimeZone)
    {      
      return  new FhirDateTimeFactory(IServerDefaultTimeZoneTimeSpan_MockFactory.Get(DefaultServerTimeZone).Object);      
    }
  }
}
