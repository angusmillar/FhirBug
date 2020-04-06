using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.R4Fhir.Serialization;
using Bug.Stu3Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Fhir
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

    public Common.FhirTools.FhirResource ParseJson(Common.Enums.FhirVersion fhirMajorVersion ,string jsonResource)
    {
      switch (fhirMajorVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          return new Common.FhirTools.FhirResource(fhirMajorVersion) { Stu3 = IStu3ParseJson.ParseJson(jsonResource) };
        case Common.Enums.FhirVersion.R4:
          return new Common.FhirTools.FhirResource(fhirMajorVersion) { R4 = IR4ParseJson.ParseJson(jsonResource) };
        default:
          throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }
  }
}
