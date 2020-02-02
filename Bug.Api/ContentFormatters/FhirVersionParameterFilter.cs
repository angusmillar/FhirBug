using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System;

namespace Bug.Api.ContentFormatters
{
  public class FhirVersionParameterFilter : IResultFilter
  {
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
      if (context != null)
      {
        if (context.Controller is Bug.Api.Controllers.FhirStu3Controller)
        {
          context.HttpContext.Items.Add("FhirVersion", "3.0");
        }
        else if (context.Controller is Bug.Api.Controllers.FhirR4Controller)
        {
          context.HttpContext.Items.Add("FhirVersion", "4.0");
        }
        else if (context.Controller is Bug.Api.Controllers.AdminController)
        {
          //nothing to do but want to check each to catch error when new FHIR version added
        }
        else
        {
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, "Unable to resolve which major version of FHIR is in use.");
        }
      }
    }
  }
}
