using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Hl7.Fhir.ElementModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Indexing
{
  public class TypedElementSupport : ITypedElementSupport
  {
    private readonly IFhirTypedElementSupportFactory IFhirTypedElementSupportFactory;
    public TypedElementSupport(IFhirTypedElementSupportFactory IFhirTypedElementSupportFactory)
    {
      this.IFhirTypedElementSupportFactory = IFhirTypedElementSupportFactory;
    }

    public IEnumerable<ITypedElement>? Select(FhirResource fhirResource, string Expression)
    {
      switch (fhirResource.FhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirTypedElementSupportFactory.GetStu3();
          return Stu3Tool.Select(fhirResource, Expression);
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirTypedElementSupportFactory.GetR4();
          return R4Tool.Select(fhirResource, Expression);
        default:
          throw new FhirVersionFatalException(fhirResource.FhirVersion);
      }
    }
  }
}
