using Bug.Common.FhirTools;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.Indexing
{
  public interface IStu3TypedElementSupport
  {
    IEnumerable<ITypedElement>? Select(IFhirResourceStu3 fhirResource, string Expression);
  }
}