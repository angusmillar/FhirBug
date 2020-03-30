using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum SearchComparator
  {
    [EnumInfo("eq", "equal to")]
    Eq = 0,
    [EnumInfo("ne", "not equal to")]
    Ne = 1,
    [EnumInfo("gt", "greater than")]
    Gt = 2,
    [EnumInfo("lt", "less than")]
    Lt = 3,
    [EnumInfo("ge", "greater or equal to")]
    Ge = 4,
    [EnumInfo("le", "less or equal to")]
    Le = 5,
    [EnumInfo("sa", "starts after")]
    Sa = 6,
    [EnumInfo("eb", "ends before")]
    Eb = 7,
    [EnumInfo("ap", "approximately the same to")]
    Ap = 8
  }
}
