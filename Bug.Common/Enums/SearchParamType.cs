using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum SearchParamType
  {
    [EnumInfo("Number", "Number")]
    Number = 0,
    [EnumInfo("Date", "Date")]
    Date = 1,
    [EnumInfo("String", "String")]
    String = 2,
    [EnumInfo("Token", "Token")]
    Token = 3,
    [EnumInfo("Reference", "Reference")]
    Reference = 4,
    [EnumInfo("Composite", "Composite")]
    Composite = 5,
    [EnumInfo("Quantity", "Quantity")]
    Quantity = 6,
    [EnumInfo("Uri", "Uri")]
    Uri = 7,
    [EnumInfo("Special", "Special")]
    Special = 8
  }
}
