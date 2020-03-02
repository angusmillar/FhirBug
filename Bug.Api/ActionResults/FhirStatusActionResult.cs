extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Bug.Logic.Query.FhirApi;

namespace Bug.Api.ActionResults
{
  public class FhirStatusActionResult : ActionResult
  {

    private FhirApiResult FhirApiResult;
    public FhirStatusActionResult(FhirApiResult FhirApiResult)
    {
      this.FhirApiResult = FhirApiResult;
    }

    public override void ExecuteResult(ActionContext context)
    {
      if (context is object)
        context.HttpContext.Response.StatusCode = (int)FhirApiResult.HttpStatusCode;      
    }


  }
}
