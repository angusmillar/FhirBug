using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum IncludeType 
  {
    [EnumInfo("_include", "Include")]
    Include,
    [EnumInfo("_revinclude", "Revinclude")]
    Revinclude 
  };
}
