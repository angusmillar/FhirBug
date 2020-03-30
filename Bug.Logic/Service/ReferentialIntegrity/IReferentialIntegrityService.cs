using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Service.ReferentialIntegrity
{
  public interface IReferentialIntegrityService
  {
    Task<ReferentialIntegrityOutcome> CheckOnCommit(FhirVersion fhirVersion, List<IndexReference> IndexReferenceList);
    Task<ReferentialIntegrityOutcome> CheckOnDelete(Common.Enums.FhirVersion fhirVersion, ResourceType resourceType, string resourceId);
  }
}