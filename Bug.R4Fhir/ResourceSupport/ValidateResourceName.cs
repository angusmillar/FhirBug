using Bug.Common.StringTools;
using Bug.Common.Exceptions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Bug.R4Fhir.ResourceSupport
{
  public class ValidateResourceName : IValidateResourceName
  {
    public bool IsKnownResource(string ResourceName)
    {
      return ModelInfo.IsKnownResource(ResourceName);
    }
  }
}
