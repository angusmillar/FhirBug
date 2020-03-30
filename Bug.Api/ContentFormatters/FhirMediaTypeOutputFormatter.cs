extern alias R4;
extern alias Stu3;
using R4Model = R4.Hl7.Fhir.Model;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;

namespace Bug.Api.ContentFormatters
{
  public abstract class FhirMediaTypeOutputFormatter : TextOutputFormatter
  {
    public FhirMediaTypeOutputFormatter() : base()
    {
      this.SupportedEncodings.Clear();
      this.SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanWriteType(Type type)
    {
      // Do we need to call the base implementation?
      // base.CanWriteType(type);
      if (type == typeof(R4Model.OperationOutcome))
        return true;
      if (typeof(R4Model.Resource).IsAssignableFrom(type))
        return true;
      if (type == typeof(Stu3Model.OperationOutcome))
        return true;
      if (typeof(Stu3Model.Resource).IsAssignableFrom(type))
        return true;
      // The null case here is to support the deleted FhirObjectResult
      if (type == null)
        return true;
      return false;
    }

    public override void WriteResponseHeaders(OutputFormatterWriteContext context)
    {
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      base.WriteResponseHeaders(context);
      if (context.Object is R4Model.Resource resourceR4)
      {
        // output the Last-Modified header using the RFC1123 format
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings?view=netframework-4.7
        //if (resourceR4.Meta != null && resourceR4.Meta.LastUpdated.HasValue)
        //  context.HttpContext.Response.Headers.Add(HeaderNames.LastModified, resourceR4.Meta.LastUpdated.Value.UtcDateTime.ToString("r", System.Globalization.CultureInfo.CurrentCulture));
        //else
        //  context.HttpContext.Response.Headers.Add(HeaderNames.LastModified, DateTimeOffset.UtcNow.ToString("r", System.Globalization.CultureInfo.CurrentCulture));
        //if (resourceR4.Meta != null && !String.IsNullOrEmpty(resourceR4.Meta.VersionId))
        //  context.HttpContext.Response.Headers.Add(HeaderNames.ETag, $"W/\"{resourceR4.Meta.VersionId}\"");
        //if (!string.IsNullOrEmpty(resourceR4.Id) && resourceR4.ResourceIdentity(resourceR4.ResourceBase) != null)
        //  context.HttpContext.Response.Headers.Add(HeaderNames.Location, resourceR4.ResourceIdentity(resourceR4.ResourceBase).OriginalString);

        if (resourceR4 is R4Model.Binary)
        {
          context.HttpContext.Response.Headers.Add(HeaderNames.ContentType, ((R4Model.Binary)resourceR4).ContentType);
          context.ContentType = new Microsoft.Extensions.Primitives.StringSegment(((R4Model.Binary)resourceR4).ContentType);
        }
      }
      else if (context.Object is Stu3Model.Resource resourceStu3)
      {
        // output the Last-Modified header using the RFC1123 format
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings?view=netframework-4.7
        //if (resourceStu3.Meta != null && resourceStu3.Meta.LastUpdated.HasValue)
        //  context.HttpContext.Response.Headers.Add(HeaderNames.LastModified, resourceStu3.Meta.LastUpdated.Value.UtcDateTime.ToString("r", System.Globalization.CultureInfo.CurrentCulture));
        //else
        //  context.HttpContext.Response.Headers.Add(HeaderNames.LastModified, DateTimeOffset.UtcNow.ToString("r", System.Globalization.CultureInfo.CurrentCulture));
        //if (resourceStu3.Meta != null && !string.IsNullOrEmpty(resourceStu3.Meta.VersionId))
        //  context.HttpContext.Response.Headers.Add(HeaderNames.ETag, $"W/\"{resourceStu3.Meta.VersionId}\"");
        //if (!string.IsNullOrEmpty(resourceStu3.Id) && resourceStu3.ResourceIdentity(resourceStu3.ResourceBase) != null)
        //  context.HttpContext.Response.Headers.Add(HeaderNames.Location, resourceStu3.ResourceIdentity(resourceStu3.ResourceBase).OriginalString);

        if (resourceStu3 is Stu3Model.Binary)
        {
          context.HttpContext.Response.Headers.Add(HeaderNames.ContentType, ((Stu3Model.Binary)resourceStu3).ContentType);
          context.ContentType = new Microsoft.Extensions.Primitives.StringSegment(((Stu3Model.Binary)resourceStu3).ContentType);
        }
      }
    }

  }
}
