using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel.Projection
{
  public class ResourceStoreNoBlob : ModelBase
  {
    public string? ResourceId { get; set; }
    public string? VersionId { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime LastUpdated { get; set; }    
  }
}
