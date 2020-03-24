using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;

namespace Bug.Logic.Service.Indexing.Setter
{
  public interface IDateTimeSetterSupport
  {
    IList<IndexDateTime> Set(FhirVersion fhirVersion, ITypedElement typedElement, ResourceType resourceType, int searchParameterId, string searchParameterName);
  }
}