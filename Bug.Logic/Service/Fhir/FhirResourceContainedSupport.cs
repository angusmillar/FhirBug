using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Fhir
{
  public class FhirResourceContainedSupport : IFhirResourceContainedSupport
  {
    private readonly IFhirContainedSupportFactory IFhirContainedSupportFactory;
    public FhirResourceContainedSupport(IFhirContainedSupportFactory IFhirContainedSupportFactory)
    {
      this.IFhirContainedSupportFactory = IFhirContainedSupportFactory;
    }

    public IList<FhirContainedResource> GetContainedResourceDictionary(FhirResource fhirResource)
    {
      switch (fhirResource.FhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var ToolStu3 = IFhirContainedSupportFactory.GetStu3();
          return ToolStu3.GetContainedResourceDictionary(fhirResource);
        case Common.Enums.FhirVersion.R4:
          var ToolR4 = IFhirContainedSupportFactory.GetR4();
          return ToolR4.GetContainedResourceDictionary(fhirResource);
        default:
          throw new FhirVersionFatalException(fhirResource.FhirVersion);
      }
    }

  }
}
