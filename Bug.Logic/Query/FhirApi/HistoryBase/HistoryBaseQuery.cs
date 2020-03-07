using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.HistoryBase
{
  
  public class HistoryBaseQuery : FhirBaseApiQuery
  {
    public HistoryBaseQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary)
      : base(HttpVerb, FhirVersion, RequestUri, HeaderDictionary) { }
  }
}
