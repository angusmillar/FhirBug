using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{  
  public enum SummaryType
  {
    [EnumInfo("True", "True")]
    True = 0,
    [EnumInfo("Text", "Text")]
    Text = 1,
    [EnumInfo("Data", "Data")]
    Data = 2,
    [EnumInfo("Count", "Count")]
    Count = 3,
    [EnumInfo("False", "False")]
    False = 4
  };
}
