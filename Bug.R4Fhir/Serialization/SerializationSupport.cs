using System;
using Bug.Common.FhirTools;
using Bug.R4.Enums;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace Bug.R4Fhir.Serialization
{
  public class SerializationSupport : IR4SerializationToJson, IR4SerializationToXml, IR4SerializationToJsonBytes
  {
    public string SerializeToXml(Resource resource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False)
    {
      SummaryTypeMap Map = new SummaryTypeMap();
      try
      {
        FhirXmlSerializer FhirXmlSerializer = new FhirXmlSerializer();
        return FhirXmlSerializer.SerializeToString(resource, Map.Get(summaryType));
      }
      catch (Exception oExec)
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, oExec.Message);
      }
    }

    public string SerializeToJson(Resource resource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False)
    {
      SummaryTypeMap Map = new SummaryTypeMap();
      try
      {
        FhirJsonSerializer FhirJsonSerializer = new FhirJsonSerializer();
        FhirJsonSerializer.Settings.Pretty = true;

        return FhirJsonSerializer.SerializeToString(resource, Map.Get(summaryType));
      }
      catch (Exception oExec)
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, oExec.Message);
      }
    }

    public byte[] SerializeToJsonBytes(IFhirResourceR4 fhirResource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False)
    {
      SummaryTypeMap Map = new SummaryTypeMap();
      try
      {
        FhirJsonSerializer FhirJsonSerializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = false, AppendNewLine = false });
        return FhirJsonSerializer.SerializeToBytes(fhirResource.R4, Map.Get(summaryType));
      }
      catch (Exception oExec)
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, oExec.Message);
      }
    }

  }
}
