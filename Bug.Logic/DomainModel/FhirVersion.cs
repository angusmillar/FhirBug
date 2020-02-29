using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Logic.DomainModel
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class FhirVersion : ModelBase
  {
    public string VersionCode { get; set; }

    public FhirMajorVersion FhirMajorVersion { get; set; }    
    //public ICollection<ResourceStore> ResourceStoreList { get; set; }
  }
}
