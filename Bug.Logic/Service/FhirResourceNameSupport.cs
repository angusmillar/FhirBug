using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class FhirResourceNameSupport : IFhirResourceNameSupport
  {
    private readonly IFhirResourceNameSupportFactory IFhirResourceNameSupportFactory;
    public FhirResourceNameSupport(IFhirResourceNameSupportFactory IFhirResourceNameSupportFactory)
    {
      this.IFhirResourceNameSupportFactory = IFhirResourceNameSupportFactory;
    }

    public string GetName(FhirResource fhirResource)
    {
      switch (fhirResource.FhirMajorVersion)
      {
        case Common.Enums.FhirMajorVersion.Stu3:
          var Stu3Tool = IFhirResourceNameSupportFactory.GetStu3();
          return Stu3Tool.GetName(fhirResource);
        case Common.Enums.FhirMajorVersion.R4:
          var R4Tool = IFhirResourceNameSupportFactory.GetR4();
          return R4Tool.GetName(fhirResource);
        default:
          throw new FhirVersionFatalException(fhirResource.FhirMajorVersion);
      }
    }
  }
}
