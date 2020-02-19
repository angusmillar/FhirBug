using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum OperationScope
  {
    [EnumInfo("Base", "Base")]
    Base,
    [EnumInfo("Resource", "Resource")]
    Resource,
    [EnumInfo("Instance", "Instance")]
    Instance
  }
}
