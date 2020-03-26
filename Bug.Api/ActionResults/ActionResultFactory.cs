using Bug.Common.ApplicationConfig;
using Bug.Logic.Query.FhirApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api.ActionResults
{
  public class ActionResultFactory : IActionResultFactory
  {
    private readonly IServiceBaseUrl IServiceBaseUrl;
    public ActionResultFactory(IServiceBaseUrl IServiceBaseUrl)
    {
      this.IServiceBaseUrl = IServiceBaseUrl;
    }

    public ActionResult GetActionResult(FhirApiResult FhirApiResult)
    {
      if (FhirApiResult is null)
        throw new ArgumentNullException(nameof(FhirApiResult));

      switch (FhirApiResult.FhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          if (FhirApiResult.FhirResource is object && FhirApiResult.FhirResource.Stu3 is object)
          {
            //When we have a FHIR Resource to return
            FhirApiResult.FhirResource.Stu3.ResourceBase = IServiceBaseUrl.Url(FhirApiResult.FhirVersion);
            return new FhirStu3ResourceActionResult(FhirApiResult.HttpStatusCode, FhirApiResult.FhirResource.Stu3);
          }
          else
          {
            //When we only have a status code to return, such as a delete or a resource not found action 
            return new FhirStatusActionResult(FhirApiResult);
          }
        case Common.Enums.FhirVersion.R4:
          if (FhirApiResult.FhirResource is object && FhirApiResult.FhirResource.R4 is object)
          {
            //When we have a FHIR Resource to return
            FhirApiResult.FhirResource.R4.ResourceBase = IServiceBaseUrl.Url(FhirApiResult.FhirVersion);
            return new FhirR4ResourceActionResult(FhirApiResult.HttpStatusCode, FhirApiResult.FhirResource.R4);
          }
          else
          {
            //When we only have a status code to return, such as a delete or a resource not found action 
            return new FhirStatusActionResult(FhirApiResult);
          }
        default:
          throw new Bug.Common.Exceptions.FhirVersionFatalException(FhirApiResult.FhirVersion);

      }
    }
  }
}
