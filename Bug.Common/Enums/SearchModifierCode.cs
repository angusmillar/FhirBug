using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum SearchModifierCode
  {
    [EnumInfo("missing", "Missing")]    
    Missing = 0,
    [EnumInfo("exact", "Exact")]
    Exact = 1,
    [EnumInfo("contains", "Contains")]
    Contains = 2,
    [EnumInfo("not", "Not")]
    Not = 3,
    [EnumInfo("text", "Text")]
    Text = 4,
    [EnumInfo("in", "In")]
    In = 5,
    [EnumInfo("not-in", "NotIn")]
    NotIn = 6,
    [EnumInfo("below", "Below")]
    Below = 7,
    [EnumInfo("above", "Above")]
    Above = 8,
    [EnumInfo("type", "Type")]
    Type = 9,
    [EnumInfo("identifier", "Identifier")]
    Identifier = 10,
    [EnumInfo("of-type", "Of-Type")]
    OfType = 11
  }
}
