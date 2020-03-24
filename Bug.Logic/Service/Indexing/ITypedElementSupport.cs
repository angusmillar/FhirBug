using Bug.Common.FhirTools;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;

namespace Bug.Logic.Service.Indexing
{
  public interface ITypedElementSupport
  {
    IEnumerable<ITypedElement>? Select(FhirResource fhirResource, string Expression);
  }
}