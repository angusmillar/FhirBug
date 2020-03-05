using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public abstract class BaseEnumKey<EnumType> : BaseDateStamp
    where EnumType : Enum
  {
    public EnumType Id { get; set; }
  }
}
