using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum UrnType
  {
    [EnumInfo("uuid", "uuid")]
    uuid,
    [EnumInfo("oid", "oid")]
    oid
  }
}
