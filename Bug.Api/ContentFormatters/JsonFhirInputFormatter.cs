extern alias Stu3;
extern alias R4;
using R4Rest = R4.Hl7.Fhir.Rest;
using R4Model = R4.Hl7.Fhir.Model;
using R4Serialization = R4.Hl7.Fhir.Serialization;
using Stu3Rest = R4.Hl7.Fhir.Rest;
using Stu3Model = R4.Hl7.Fhir.Model;
using Stu3Serialization = R4.Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bug.Common.Exceptions;

namespace Bug.Api.ContentFormatters
{
  public class JsonFhirInputFormatter : FhirMediaTypeInputFormatter
  {
    public JsonFhirInputFormatter() : base()
    {
      foreach (var mediaType in R4Rest.ContentType.JSON_CONTENT_HEADERS)
        SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      if (encoding == null)
        throw new ArgumentNullException(nameof(encoding));
      
      if (encoding.EncodingName != Encoding.UTF8.EncodingName)
      {        
        throw new FhirFatalException(System.Net.HttpStatusCode.BadRequest, "FHIR supports UTF-8 encoding exclusively, not " + encoding.WebName);
      }
        

      var request = context.HttpContext.Request;

      // TODO: Brian: Would like to know what the issue is here? Will this be resolved by the Async update to the core?
      if (!request.Body.CanSeek)
      {
        // To avoid blocking on the stream, we asynchronously read everything 
        // into a buffer, and then seek back to the beginning.
        request.EnableBuffering();
        Debug.Assert(request.Body.CanSeek);

        // no timeout configuration on this? or does that happen at another layer?
        await request.Body.DrainAsync(CancellationToken.None);
        request.Body.Seek(0L, SeekOrigin.Begin);
      }

      using (var streamReader = context.ReaderFactory(request.Body, encoding))
      using (var jsonReader = new JsonTextReader(streamReader))
      {
        // need to configure these properties as is done in 
        // HL7.Fhir.SerializationUtil.JsonReaderFromJsonText()
        jsonReader.DateParseHandling = DateParseHandling.None;
        jsonReader.FloatParseHandling = FloatParseHandling.Decimal;

        try
        {
          if (FhirMajorVersion == Common.Enums.FhirVersion.Stu3)
          {
            var resource = new R4Serialization.FhirJsonParser().Parse<R4Model.Resource>(jsonReader);
            return InputFormatterResult.Success(resource);
          }
          else if (FhirMajorVersion == Common.Enums.FhirVersion.R4)
          {
            var resource = new Stu3Serialization.FhirJsonParser().Parse<Stu3Model.Resource>(jsonReader);
            return InputFormatterResult.Success(resource);
          }
          else
          {
            throw new FhirFatalException(System.Net.HttpStatusCode.BadRequest, "Unable to resolve which major version of FHIR is in use.");
          }
        }
        catch (FormatException exception)
        {
          throw new FhirFatalException(System.Net.HttpStatusCode.BadRequest, "Body parsing failed: " + exception.Message);
        }
      }

    }
  }
}
