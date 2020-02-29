using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum FhirMajorVersion 
  {  
    [EnumInfo("Stu3", "Stu 3 Sequence")]
    Stu3,
    [EnumInfo("R4", "R4 Sequence")]
    R4    
  };
}
