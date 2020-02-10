using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Logic.Command.FhirApi
{
  public class FhirApiOutcome
  {
    public HttpStatusCode httpStatusCode { get; set;  }
    public object resource { get; set; }    
    public FhirMajorVersion fhirMajorVersion { get; set; }

  }
}
