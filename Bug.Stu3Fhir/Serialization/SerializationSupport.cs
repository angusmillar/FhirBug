using System;
using System.IO;
using Bug.Common.FhirTools;
using Bug.Stu3Fhir.Enums;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Newtonsoft.Json;

namespace Bug.Stu3Fhir.Serialization
{
  public class SerializationSupport : IStu3SerializationToJsonBytes, IStu3SerializationToJson, IStu3SerializationToXml, IStu3ParseJson
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

    public Resource ParseJson(string jsonResource)
    {
      SummaryTypeMap Map = new SummaryTypeMap();
      try
      {
        FhirJsonParser FhirJsonParser = new FhirJsonParser();
        return FhirJsonParser.Parse<Resource>(jsonResource);
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

    public byte[] SerializeToJsonBytes(IFhirResourceStu3 fhirResource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False)
    {
      SummaryTypeMap Map = new SummaryTypeMap();
      try
      {
        FhirJsonSerializer FhirJsonSerializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = false, AppendNewLine = false });
        return FhirJsonSerializer.SerializeToBytes(fhirResource.Stu3, Map.Get(summaryType));
      }
      catch (Exception oExec)
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, oExec.Message);
      }
    }

  }
}
