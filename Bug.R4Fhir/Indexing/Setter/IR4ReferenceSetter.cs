using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.R4Fhir.Indexing.Setter
{ 
  public interface IR4ReferenceSetter
  {
    Task<IList<IndexReference>> SetAsync(ITypedElement typedElement, ResourceType resourceType, int searchParameterId, string searchParameterName);
  }
}