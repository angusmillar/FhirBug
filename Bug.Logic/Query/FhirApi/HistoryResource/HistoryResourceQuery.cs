using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.HistoryResource
{
  
  public class HistoryResourceQuery : FhirApiResourceQuery
  {
    public HistoryResourceQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName)
      : base(HttpVerb, FhirVersion, RequestUri, HeaderDictionary, ResourceName) { }
  }
}
