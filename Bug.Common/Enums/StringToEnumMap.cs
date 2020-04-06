using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Common.Enums
{
  public static class StringToEnumMap<EnumType> where EnumType : Enum
  {
    public static Dictionary<string, EnumType> GetDictionary()
    {
      return Enum.GetValues(typeof(EnumType)).Cast<EnumType>().ToDictionary(x => x.GetCode(), y => y);            
    }
  }
}
