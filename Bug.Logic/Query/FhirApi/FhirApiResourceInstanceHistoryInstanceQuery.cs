using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirApiResourceInstanceHistoryInstanceQuery : FhirApiResourceInstanceQuery
  {
    public FhirApiResourceInstanceHistoryInstanceQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId, int VersionId)
      : base(HttpVerb, FhirVersion, RequestUri, HeaderDictionary, ResourceName, ResourceId)
    {
      this.VersionId = VersionId;      
    }    
    public int VersionId { get; set; }    
  }
}
