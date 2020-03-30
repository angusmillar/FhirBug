using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum SearchModifierCode
  {
    [EnumInfo("Missing", "Missing")]    
    Missing = 0,
    [EnumInfo("Exact", "Exact")]
    Exact = 1,
    [EnumInfo("Contains", "Contains")]
    Contains = 2,
    [EnumInfo("Not", "Not")]
    Not = 3,
    [EnumInfo("Text", "Text")]
    Text = 4,
    [EnumInfo("In", "In")]
    In = 5,
    [EnumInfo("NotIn", "NotIn")]
    NotIn = 6,
    [EnumInfo("Below", "Below")]
    Below = 7,
    [EnumInfo("Above", "Above")]
    Above = 8,
    [EnumInfo("Type", "Type")]
    Type = 9,
    [EnumInfo("Identifier", "Identifier")]
    Identifier = 10,
    [EnumInfo("OfType", "OfType")]
    OfType = 11
  }
}
