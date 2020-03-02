using Bug.Logic.Query.FhirApi;
using Microsoft.AspNetCore.Mvc;

namespace Bug.Api.ActionResults
{
  public interface IActionResultFactory
  {
    ActionResult Get(FhirApiResult FhirApiResult);
  }
}