using System;

namespace Bug.Common.DateTimeTools
{
  public interface IServerDateTimeSupport
  {
    DateTimeOffset Now();
    DateTimeOffset ZuluToServerTimeZone(DateTime zuluDateTime);
  }
}