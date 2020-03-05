using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class ResourceName : BaseIntKey
  {
    public string Name { get; set; }    
    //public ICollection<ResourceStore> ResourceStoreList { get; set; }
  }
}
