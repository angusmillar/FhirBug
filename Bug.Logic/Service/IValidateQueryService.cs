using Bug.Common.FhirTools;
using Bug.Logic.Query.FhirApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public interface IValidateQueryService
  {
    bool IsValid(FhirApiResourceQuery fhirApiResourceQuery, out FhirResource? OperationOutCome);
  }
}
