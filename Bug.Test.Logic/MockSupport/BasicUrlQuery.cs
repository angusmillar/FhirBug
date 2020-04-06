using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public class BasicUrlQuery
  {
    public BasicUrlQuery()
    {
      
    }

    public Dictionary<string, StringValues> QueryDictionary { get; set; }
  }
}
