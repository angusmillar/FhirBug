using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Indexing.Setter
{
  public interface IReferenceSetterSupport
  {
    Task<IList<Bug.Common.Dto.Indexing.IndexReference>> SetAsync(FhirVersion fhirVersion, ITypedElement typedElement, ResourceType resourceType, int searchParameterId, string searchParameterName);
  }
}