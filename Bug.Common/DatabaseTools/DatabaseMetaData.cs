using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.DatabaseTools
{
  public static class DatabaseMetaData
  {
    public static class FieldLength
    {
      public const int ResourceTypeStringMaxLength =  50;
      public const int FhirIdMaxLength = 128;
      public const int StringMaxLength = 450; 
      public const int NameMaxLength = 128; 
      public const int DescriptionMaxLength = 256; 
      public const byte QuantityPrecision = 28; 
      public const byte QuantityScale = 14; 
      public const int CodeMaxLength = 128; 
      public const int UnitMaxLength = 64; 
      public const byte DateTimeOffsetPrecision = 3; 
    }
  }
}
