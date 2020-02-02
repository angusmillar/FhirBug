extern alias R4;
extern alias Stu3;
using R4Rest = R4.Hl7.Fhir.Rest;
using R4Model = R4.Hl7.Fhir.Model;
using R4Serialization = R4.Hl7.Fhir.Serialization;
using Stu3Rest = Stu3.Hl7.Fhir.Rest;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Stu3Serialization = Stu3.Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Xml;


namespace Bug.Api.ContentFormatters
{
  public class XmlFhirOutputFormatter : FhirMediaTypeOutputFormatter
  {
    public XmlFhirOutputFormatter() : base()
    {
      foreach (var mediaType in R4Rest.ContentType.XML_CONTENT_HEADERS)
        SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
    }

    public override void WriteResponseHeaders(OutputFormatterWriteContext context)
    {
      context.ContentType = FhirMediaType.GetMediaTypeHeaderValue(context.ObjectType, Bug.Common.Enums.FhirFormatType.xml);
      // note that the base is called last, as this may overwrite the ContentType where the resource is of type Binary
      base.WriteResponseHeaders(context);
      //   headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "fhir.resource.json" };
    }

    public override System.Threading.Tasks.Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      if (selectedEncoding == null)
        throw new ArgumentNullException(nameof(selectedEncoding));

      XmlWriterSettings settings = new XmlWriterSettings
      {
        Encoding = new UTF8Encoding(false),
        OmitXmlDeclaration = true,
        Async = true,
        CloseOutput = true,
        Indent = true,
        NewLineHandling = NewLineHandling.Entitize,
        IndentChars = "  "
      };

      using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Body, settings))
      {
        R4Rest.SummaryType R4SummaryType = R4Rest.SummaryType.False;
        Stu3Rest.SummaryType Stu3SummaryType = Stu3Rest.SummaryType.False;
        if (context.ObjectType == typeof(R4Model.OperationOutcome))
        {
          // We will only honor the summary type during serialization of the outcome
          // if the resource wasn't a stored OpOutcome we are returning
          R4Model.OperationOutcome resource = (R4Model.OperationOutcome)context.Object;
          if (string.IsNullOrEmpty(resource.Id) && resource.HasAnnotation<R4Rest.SummaryType>())
            R4SummaryType = resource.Annotation<R4Rest.SummaryType>();
          new R4Serialization.FhirXmlSerializer().Serialize(resource, writer, R4SummaryType);
        }
        else if (context.ObjectType == typeof(Stu3Model.OperationOutcome))
        {
          // We will only honor the summary type during serialization of the outcome
          // if the resource wasn't a stored OpOutcome we are returning
          Stu3Model.OperationOutcome resource = (Stu3Model.OperationOutcome)context.Object;
          if (string.IsNullOrEmpty(resource.Id) && resource.HasAnnotation<Stu3Rest.SummaryType>())
            Stu3SummaryType = resource.Annotation<Stu3Rest.SummaryType>();
          new Stu3Serialization.FhirXmlSerializer().Serialize(resource, writer, Stu3SummaryType);
        }
        else if (typeof(R4Model.Resource).IsAssignableFrom(context.ObjectType))
        {          
          if (context.Object != null)
          {
            R4Model.Resource r = context.Object as R4Model.Resource;
            if (r.HasAnnotation<R4Rest.SummaryType>())
              R4SummaryType = r.Annotation<R4Rest.SummaryType>();

            new R4Serialization.FhirXmlSerializer().Serialize(r, writer, R4SummaryType);
          }
        }
        else if (typeof(Stu3Model.Resource).IsAssignableFrom(context.ObjectType))
        {
          if (context.Object != null)
          {
            Stu3Model.Resource r = context.Object as Stu3Model.Resource;
            if (r.HasAnnotation<Stu3Rest.SummaryType>())
              Stu3SummaryType = r.Annotation<Stu3Rest.SummaryType>();
            new Stu3Serialization.FhirXmlSerializer().Serialize(r, writer, Stu3SummaryType);
          }
        }
        return writer.FlushAsync();
      }
    }
  }
}
