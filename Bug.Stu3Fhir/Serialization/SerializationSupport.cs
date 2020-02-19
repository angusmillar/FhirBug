﻿using System;
using System.IO;
using Bug.Stu3Fhir.Enums;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Newtonsoft.Json;

namespace Bug.Stu3Fhir.Serialization
{
  public class SerializationSupport : IStu3SerializationToJsonBytes, IStu3SerializationToJson, IStu3SerializationToXml
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

    public byte[] SerializeToJsonBytes(object resource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False)
    {
      if (resource is Resource Res)
      {
        SummaryTypeMap Map = new SummaryTypeMap();
        try
        {
          FhirJsonSerializer FhirJsonSerializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = false, AppendNewLine = false });
          return FhirJsonSerializer.SerializeToBytes(Res, Map.Get(summaryType));
        }
        catch (Exception oExec)
        {
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, oExec.Message);
        }
      }
      else
      {
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, "Casting error, unable to cast object to Stu3 Resource in method SerializeToJsonBytes.");
      }
    }

  }
}