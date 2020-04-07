using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.DecimalTools
{
  public static class DecimalSupport
  {
    public static DecimalInfo GetDecimalInfo(decimal dec)
    {
      var x = new System.Data.SqlTypes.SqlDecimal(dec);      
      return new DecimalInfo((int)x.Precision, (int)x.Scale);
    }

    public class DecimalInfo
    {
      public DecimalInfo(int Precision, int Scale)
      {        
        this.Precision = Precision;
        this.Scale = Scale;
      }      
      public int Precision { get; private set; }
      public int Scale { get; private set; }
    }
      

  }
}
