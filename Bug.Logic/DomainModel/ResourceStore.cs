using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class ResourceStore : ModelBase
  {
    public string ResourceId { get; set; }
    public int VersionId { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime LastUpdated { get; set; }
    public byte[]? ResourceBlob { get; set; }
    public int FkResourceNameId { get; set; }
    public ResourceName ResourceName { get; set; }
    public int FkFhirVersionId { get; set; }
    public FhirVersion FhirVersion { get; set; }
    public int FkMethodId { get; set; }
    public Method Method { get; set; }

  }
}
