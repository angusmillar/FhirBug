using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.R4Fhir.Serialization;
using Bug.Stu3Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class FhirResourceParseJsonService : IFhirResourceParseJsonService
  {
    private readonly IStu3ParseJson IStu3ParseJson;
    private readonly IR4ParseJson IR4ParseJson;
    public FhirResourceParseJsonService(IStu3ParseJson IStu3ParseJson,
      IR4ParseJson IR4ParseJson)
    {
      this.IStu3ParseJson = IStu3ParseJson;
      this.IR4ParseJson = IR4ParseJson;
    }

    public FhirResource ParseJson(FhirMajorVersion fhirMajorVersion ,string jsonResource)
    {
      switch (fhirMajorVersion)
      {
        case FhirMajorVersion.Stu3:
          return new FhirResource(fhirMajorVersion) { Stu3 = IStu3ParseJson.ParseJson(jsonResource) };
        case FhirMajorVersion.R4:
          return new FhirResource(fhirMajorVersion) { R4 = IR4ParseJson.ParseJson(jsonResource) };
        default:
          throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }
  }
}
