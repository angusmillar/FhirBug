using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public abstract class BaseEnumKey<EnumType> : ModelBase
    where EnumType : Enum
  {
    public EnumType Id { get; set; }
  }
}
