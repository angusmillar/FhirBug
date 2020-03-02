using Bug.Common.FhirTools;
using Bug.Logic.Query.FhirApi;

namespace Bug.Logic.Service.ValidatorService
{
  public interface IValidateQueryService
  {
    bool IsValid(FhirBaseApiQuery fhirApiQuery, out FhirResource? OperationOutCome);
  }
}
