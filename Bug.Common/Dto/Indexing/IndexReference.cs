using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Common.Dto.Indexing
{
  public class IndexReference : IndexBase
  {
    public IndexReference(int fkSearchParameterId) 
      : base(fkSearchParameterId){ }

    public int? ServiceBaseUrlId { get; set; }
    public ServiceBaseUrl? ServiceBaseUrl { get; set; }    
    public ResourceType? ResourceTypeId { get; set; }    
    public string? ResourceId { get; set; }
    public string? VersionId { get; set; }
  }
}
