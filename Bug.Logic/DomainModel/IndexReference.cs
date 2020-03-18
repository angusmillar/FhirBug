using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class IndexReference : IndexBase
  {
    public int? FkServiceBaseUrlId { get; set; }
    public ServiceBaseUrl ServiceBaseUrl { get; set; }
    public Bug.Common.Enums.ResourceType FkResourceTypeId { get; set; }
    public ResourceType ResourceType { get; set; }
    public string ResourceId { get; set; }
    public string VersionId { get; set; }
  }
}
