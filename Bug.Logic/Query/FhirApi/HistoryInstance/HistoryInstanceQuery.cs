﻿using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.HistoryInstance
{  
  public class HistoryInstanceQuery : FhirApiResourceInstanceQuery
  {
    public HistoryInstanceQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> RequestQuery, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId)
      : base(HttpVerb, FhirVersion, RequestUri, RequestQuery, HeaderDictionary, ResourceName, ResourceId) { }
  }
}
