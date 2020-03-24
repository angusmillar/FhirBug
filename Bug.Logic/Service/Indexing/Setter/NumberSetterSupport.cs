using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Hl7.Fhir.ElementModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Indexing.Setter
{
  public class NumberSetterSupport : INumberSetterSupport
  {
    private readonly IFhirIndexNumberSetterSupportFactory IFhirIndexNumberSetterSupportFactory;
    public NumberSetterSupport(IFhirIndexNumberSetterSupportFactory IFhirIndexNumberSetterSupportFactory)
    {
      this.IFhirIndexNumberSetterSupportFactory = IFhirIndexNumberSetterSupportFactory;
    }

    public IList<Bug.Common.Dto.Indexing.IndexQuantity> Set(Bug.Common.Enums.FhirVersion fhirVersion, ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      switch (fhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirIndexNumberSetterSupportFactory.GetStu3();
          return Stu3Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirIndexNumberSetterSupportFactory.GetR4();
          return R4Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        default:
          throw new FhirVersionFatalException(fhirVersion);
      }
    }
  }
}
