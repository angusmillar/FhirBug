using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.Indexing.Setter
{
  public interface IStu3ReferenceSetter
  {
    System.Threading.Tasks.Task<IList<IndexReference>> SetAsync(ITypedElement typedElement, ResourceType resourceType, int searchParameterId, string searchParameterName);
  }
}