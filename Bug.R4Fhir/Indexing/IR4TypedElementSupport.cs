using Bug.Common.FhirTools;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;

namespace Bug.R4Fhir.Indexing
{
  public interface IR4TypedElementSupport
  {    
    IEnumerable<ITypedElement>? Select(IFhirResourceR4 fhirResource, string Expression);
  }
}