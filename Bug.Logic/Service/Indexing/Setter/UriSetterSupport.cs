using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Hl7.Fhir.ElementModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Indexing.Setter
{
  public class UriSetterSupport : IUriSetterSupport
  {
    private readonly IFhirIndexUriSetterSupportFactory IFhirIndexUriSetterSupportFactory;
    public UriSetterSupport(IFhirIndexUriSetterSupportFactory IFhirIndexUriSetterSupportFactory)
    {
      this.IFhirIndexUriSetterSupportFactory = IFhirIndexUriSetterSupportFactory;
    }

    public IList<Bug.Common.Dto.Indexing.IndexUri> Set(Bug.Common.Enums.FhirVersion fhirVersion, ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      switch (fhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirIndexUriSetterSupportFactory.GetStu3();
          return Stu3Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirIndexUriSetterSupportFactory.GetR4();
          return R4Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        default:
          throw new FhirVersionFatalException(fhirVersion);
      }
    }
  }
}
