using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class FhirResourceLastUpdatedSupport : IFhirResourceLastUpdatedSupport
  {
    private readonly IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory;
    public FhirResourceLastUpdatedSupport(IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory)
    {
      this.IFhirResourceLastUpdatedSupportFactory = IFhirResourceLastUpdatedSupportFactory;
    }

    public DateTimeOffset? GetLastUpdated(FhirResource fhirResource)
    {
      switch (fhirResource.FhirMajorVersion)
      {
        case Common.Enums.FhirMajorVersion.Stu3:
          var Stu3Tool = IFhirResourceLastUpdatedSupportFactory.GetStu3();
          return Stu3Tool.GetLastUpdated(fhirResource);
        case Common.Enums.FhirMajorVersion.R4:
          var R4Tool = IFhirResourceLastUpdatedSupportFactory.GetR4();
          return R4Tool.GetLastUpdated(fhirResource);
        default:
          throw new FhirVersionFatalException(fhirResource.FhirMajorVersion);
      }
    }

    public void SetLastUpdated(FhirResource fhirResource, DateTimeOffset dateTime)
    {
      switch (fhirResource.FhirMajorVersion)
      {
        case Common.Enums.FhirMajorVersion.Stu3:
          var Stu3Tool = IFhirResourceLastUpdatedSupportFactory.GetStu3();
          Stu3Tool.SetLastUpdated(dateTime, fhirResource);
          break;
        case Common.Enums.FhirMajorVersion.R4:
          var R4Tool = IFhirResourceLastUpdatedSupportFactory.GetR4();
          R4Tool.SetLastUpdated(dateTime, fhirResource);
          break;
        default:
          throw new FhirVersionFatalException(fhirResource.FhirMajorVersion);
      }
    }
  }
}
