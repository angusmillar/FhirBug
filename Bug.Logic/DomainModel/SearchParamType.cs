using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{
  public class SearchParamType : BaseEnumKey<Bug.Common.Enums.SearchParamType>
  {    
    public string Code { get; set; }
  }
}
