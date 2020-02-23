using Bug.Common.StringTools;
using Bug.Common.Exceptions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public class ValidateResourceName : IStu3ValidateResourceName
  {
    public bool IsKnownResource(string ResourceName)
    {
      return ModelInfo.IsKnownResource(ResourceName);
    }
  }
}
