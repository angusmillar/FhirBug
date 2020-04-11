using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum PreferHandlingType 
  {
    [EnumInfo("strict", "Strict")]
    Strict,
    [EnumInfo("lenient", "Lenient")]
    Lenient 
  };
}
