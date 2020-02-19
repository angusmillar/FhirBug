using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public class ResourceStore : ModelBase
  {    
    public string ResourceId { get; set; }
    public string VersionId { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime LastUpdated { get; set; }
    public byte[] ResourceBlob { get; set; }
    //public FhirVersion FhirVersion { get; set; }

  }
}
