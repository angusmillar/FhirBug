using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Logic.DomainModel
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class FhirVersion : BaseEnumKey<Common.Enums.FhirVersion>
  {
    public string Code { get; set; }   
  }
}
