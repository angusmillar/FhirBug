using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.VRead
{
  
  public class VReadQuery : FhirApiResourceInstanceHistoryInstanceQuery
  {
    public VReadQuery(HttpVerb HttpVerb, FhirMajorVersion FhirMajorVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId, int VersionId)
      : base(HttpVerb, FhirMajorVersion, RequestUri, HeaderDictionary, ResourceName, ResourceId, VersionId) { }
  }
}
