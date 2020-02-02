extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Bug.Api
{
  public class FhirActionResult : ObjectResult
  {
    private HttpStatusCode httpStatusCode;
    private Stu3Model.Resource resource;

    public FhirActionResult(HttpStatusCode httpStatusCode,  Stu3Model.Resource resource)
      :base(resource)
    {
      this.httpStatusCode = httpStatusCode;
      this.resource = resource;
    }    
  }
}
