extern alias R4;
extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using R4Model = R4.Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;

namespace Bug.Api.ContentFormatters
{
  public abstract class FhirMediaTypeInputFormatter : TextInputFormatter
  {
    public FhirMediaTypeInputFormatter() : base()
    {
      this.SupportedEncodings.Clear();
      this.SupportedEncodings.Add(UTF8EncodingWithoutBOM); // Encoding.UTF8);
    }

    /// <summary>
    /// This is set by the actual formatter (xml or json)
    /// </summary>
    //protected R4Model.Resource entry = null;
    private Common.Enums.FhirMajorVersion _FhirMajorVersion = Common.Enums.FhirMajorVersion.Stu3;
    protected Common.Enums.FhirMajorVersion FhirMajorVersion
    {
      get
      {
        return _FhirMajorVersion;
      }
      set
      {
        _FhirMajorVersion = value;
      }
    }

    protected override bool CanReadType(Type type)
    {
      if (typeof(Stu3Model.Resource).IsAssignableFrom(type))
      {
        FhirMajorVersion = Common.Enums.FhirMajorVersion.Stu3;
        return true;
      }

      if (typeof(R4Model.Resource).IsAssignableFrom(type))
      {
        FhirMajorVersion = Common.Enums.FhirMajorVersion.R4;
        return true;        
      }
        
      return false;
    }

  }
}
