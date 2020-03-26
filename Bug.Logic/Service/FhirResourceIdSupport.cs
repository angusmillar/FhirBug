using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class FhirResourceIdSupport : IFhirResourceIdSupport
  {
    private readonly IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory;
    public FhirResourceIdSupport(IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory)
    {
      this.IFhirResourceIdSupportFactory = IFhirResourceIdSupportFactory;
    }

    public string? GetResourceId(FhirResource fhirResource)
    {
      switch (fhirResource.FhirMajorVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();
          return Stu3FhirResourceIdSupport.GetFhirId(fhirResource);
        case Common.Enums.FhirVersion.R4:
          var R4FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetR4();
          return R4FhirResourceIdSupport.GetFhirId(fhirResource);
        default:
          throw new FhirVersionFatalException(fhirResource.FhirMajorVersion);
      }
    }

    public void SetResourceId(FhirResource fhirResource, string id)
    {
      switch (fhirResource.FhirMajorVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();
          Stu3FhirResourceIdSupport.SetFhirId(id, fhirResource);
          break;
        case Common.Enums.FhirVersion.R4:
          var R4FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetR4();
          R4FhirResourceIdSupport.SetFhirId(id, fhirResource);
          break;
        default:
          throw new FhirVersionFatalException(fhirResource.FhirMajorVersion);
      }
    }
  }
}
