using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel.Projection
{
  public class ReferentialIntegrityQuery
  {
    public Bug.Common.Enums.ResourceType TargetResourceTypeId { get; set; }
    public string TargetResourceId { get; set; }
    public Bug.Common.Enums.FhirVersion FhirVersionId { get; set; }
    public int? ServiceBaseUrlId { get; set; }
    public Bug.Common.Enums.ResourceType ResourceTypeId { get; set; }
    public string ResourceId { get; set; }
    public string VersionId { get; set; }    

  }
}
