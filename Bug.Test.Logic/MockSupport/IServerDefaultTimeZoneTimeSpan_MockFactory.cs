using Bug.Common.ApplicationConfig;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class IServerDefaultTimeZoneTimeSpan_MockFactory
  {
    public static Mock<IServerDefaultTimeZoneTimeSpan> Get(TimeSpan ServerDefaultTimeZone)
    {
      Mock<IServerDefaultTimeZoneTimeSpan> IServerDefaultTimeZoneTimeSpanMock = new Mock<IServerDefaultTimeZoneTimeSpan>();
      IServerDefaultTimeZoneTimeSpanMock.Setup(x => x.ServerDefaultTimeZoneTimeSpan).Returns(ServerDefaultTimeZone);     
      return IServerDefaultTimeZoneTimeSpanMock;
    }
  }
}
