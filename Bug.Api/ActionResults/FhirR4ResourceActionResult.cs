extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Bug.Api.ActionResults
{
  public class FhirR4ResourceActionResult : ObjectResult
  {
    private HttpStatusCode httpStatusCode;
    private R4Model.Resource resource;

    public FhirR4ResourceActionResult(HttpStatusCode httpStatusCode,  R4Model.Resource resource)
      :base(resource)
    {
      this.httpStatusCode = httpStatusCode;
      this.resource = resource;
    }    
  }
}
