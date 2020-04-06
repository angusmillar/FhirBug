using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public abstract class SearchQueryValueBase
  {
    protected SearchQueryValueBase(bool IsMissing)
    {
      this.IsMissing = IsMissing;
    }

    public bool IsMissing { get; set; }
    public static bool? ParseModifierEqualToMissing(string Value)
    {      
      if (Boolean.TryParse(Value, out bool ParsedBooleanValue))
      {
        return ParsedBooleanValue;
      }
      else
      {
        return null;
      }
    }
  }
}
