using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Hl7.Fhir.ElementModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Indexing.Setter
{
  public class QuantitySetterSupport : IQuantitySetterSupport
  {
    private readonly IFhirIndexQuantitySetterSupportFactory IFhirIndexQuantitySetterSupportFactory;
    public QuantitySetterSupport(IFhirIndexQuantitySetterSupportFactory IFhirIndexQuantitySetterSupportFactory)
    {
      this.IFhirIndexQuantitySetterSupportFactory = IFhirIndexQuantitySetterSupportFactory;
    }

    public IList<Bug.Common.Dto.Indexing.IndexQuantity> Set(Bug.Common.Enums.FhirVersion fhirVersion, ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      switch (fhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirIndexQuantitySetterSupportFactory.GetStu3();
          return Stu3Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirIndexQuantitySetterSupportFactory.GetR4();
          return R4Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        default:
          throw new FhirVersionFatalException(fhirVersion);
      }
    }
  }
}
