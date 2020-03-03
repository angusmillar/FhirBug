﻿using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.Read
{
  
  public class ReadQuery : FhirApiResourceInstanceQuery
  {
    public ReadQuery(HttpVerb HttpVerb, FhirMajorVersion FhirMajorVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId)
      : base(HttpVerb, FhirMajorVersion, RequestUri, HeaderDictionary, ResourceName, ResourceId) { }
  }
}