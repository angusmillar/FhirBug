using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace Bug.R4Fhir
{
  public static class SerializationSupport
  {
    public static string SerializeToXml(Resource resource, Hl7.Fhir.Rest.SummaryType summaryType = Hl7.Fhir.Rest.SummaryType.False)
    {
      try
      {
        FhirXmlSerializer FhirXmlSerializer = new FhirXmlSerializer();
        return FhirXmlSerializer.SerializeToString(resource, summaryType);
      }
      catch (Exception oExec)
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, oExec.Message);
      }
    }

    public static string SerializeToJson(Resource resource, Hl7.Fhir.Rest.SummaryType summaryType = Hl7.Fhir.Rest.SummaryType.False)
    {
      try
      {
        FhirJsonSerializer FhirJsonSerializer = new FhirJsonSerializer();
        FhirJsonSerializer.Settings.Pretty = true;

        return FhirJsonSerializer.SerializeToString(resource, summaryType);
      }
      catch (Exception oExec)
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, oExec.Message);
      }
    }

  }
}
