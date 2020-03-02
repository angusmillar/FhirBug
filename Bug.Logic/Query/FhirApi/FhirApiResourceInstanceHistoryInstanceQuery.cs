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
    public FhirApiResourceInstanceHistoryInstanceQuery(HttpVerb HttpVerb, FhirMajorVersion FhirMajorVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId, int VersionId)
      : base(HttpVerb, FhirMajorVersion, RequestUri, HeaderDictionary, ResourceName, ResourceId)
    {
      this.VersionId = VersionId;      
    }    
    public int VersionId { get; set; }    
  }
}
