using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum SortOrder
  {
    [EnumInfo("asc", "Ascending")]    
    Ascending = 0,
    [EnumInfo("desc", "Descending")]    
    Descending = 1
  }
}
