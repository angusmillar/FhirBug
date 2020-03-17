using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class IndexReference : IndexBase
  {
    public string ResourceId { get; set; }
    public string ResourceType { get; set; }
    public int? ServiceBaseUrl { get; set; }
    //public _ServiceBaseUrl ReferenceUrl { get; set; }
    public string VersionId { get; set; }
  }
}
