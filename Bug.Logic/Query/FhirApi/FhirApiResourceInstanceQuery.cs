﻿using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirApiResourceInstanceQuery : FhirApiResourceQuery
  {
    public FhirApiResourceInstanceQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId)
      : base(HttpVerb, FhirVersion, RequestUri, HeaderDictionary, ResourceName)
    {
      this.ResourceId = ResourceId;      
    }    
    public string ResourceId { get; set; }    
  }
}
