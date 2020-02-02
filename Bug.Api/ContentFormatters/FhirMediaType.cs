extern alias Stu3;
extern alias R4;
using R4Rest = R4.Hl7.Fhir.Rest;
using R4Model = R4.Hl7.Fhir.Model;
using Stu3Rest = Stu3.Hl7.Fhir.Rest;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Bug.Common.Exceptions;

namespace Bug.Api.ContentFormatters
{
  public static class FhirMediaType
  {
    // TODO: This class can be merged into HL7.Fhir.ContentType

    public const string XmlResource = "application/fhir+xml";
    public const string JsonResource = "application/fhir+json";
    public const string BinaryResource = "application/fhir+binary";
 
    public static Common.Enums.FhirFormatType GetFhirFormatTypeFromAcceptHeader(string acceptHeaderValue)
    {
      var DefaultType = Common.Enums.FhirFormatType.json;
      if (string.IsNullOrWhiteSpace(acceptHeaderValue))
        return DefaultType;

      Dictionary<string, Common.Enums.FhirFormatType> mediaTypeDic = new Dictionary<string, Common.Enums.FhirFormatType>();
      foreach (var mediaType in R4Rest.ContentType.XML_CONTENT_HEADERS)
        mediaTypeDic.Add(mediaType, Common.Enums.FhirFormatType.xml);

      foreach (var mediaType in R4Rest.ContentType.JSON_CONTENT_HEADERS)
        mediaTypeDic.Add(mediaType, Common.Enums.FhirFormatType.json);

      acceptHeaderValue = acceptHeaderValue.Trim();
      if (mediaTypeDic.ContainsKey(acceptHeaderValue))
      {
        return mediaTypeDic[acceptHeaderValue];
      }        
      else
      {
        return DefaultType;
      }
    }

    public static string GetContentType(Type type, Bug.Common.Enums.FhirFormatType format)
    {
      if (typeof(Stu3Model.Resource).IsAssignableFrom(type) || typeof(R4Model.Resource).IsAssignableFrom(type))
      {
        switch (format)
        {
          case Bug.Common.Enums.FhirFormatType.json: return JsonResource;
          case Bug.Common.Enums.FhirFormatType.xml: return XmlResource;
          default: return XmlResource;
        }
      }
      else
        return "application/octet-stream";
    }

    public static StringSegment GetMediaTypeHeaderValue(Type type, Bug.Common.Enums.FhirFormatType format)
    {
      string mediatype = FhirMediaType.GetContentType(type, format);
      MediaTypeHeaderValue header = new MediaTypeHeaderValue(mediatype);
      header.CharSet = Encoding.UTF8.WebName;
      
      if (typeof(Stu3Model.Resource).IsAssignableFrom(type))
      {        
        return new StringSegment(header.ToString() + "; FhirVersion=3.0");
      }
      else if (typeof(R4Model.Resource).IsAssignableFrom(type))
      {
        return  new StringSegment(header.ToString() + "; FhirVersion=4.0");
      }
      else
      {
        throw new FhirFatalException(System.Net.HttpStatusCode.BadRequest, "Unable to resolve which major version of FHIR is in use.");
      }
            
    }
  }
}
