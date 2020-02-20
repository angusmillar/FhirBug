extern alias Stu3;
extern alias R4;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using R4Model = R4.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Logic.Query.FhirApi.Create
{
  public class FhirResource
  {
    private Stu3Model.Resource _Stu3;
    public Stu3Model.Resource Stu3
    {
      get
      {
        if (this.R4 != null)
        {
          string message = $"Internal server error, attempted to get a FhirResource object instances' {Common.Enums.FhirMajorVersion.Stu3.GetLiteral()} property when the {Common.Enums.FhirMajorVersion.R4.GetLiteral()} resource property was already set.";
          throw new Bug.Common.Exceptions.FhirVersionFatalException(message);
        }
        return _Stu3;
      }
      set
      {
        if (this.R4 != null)
        {
          string message = $"Internal server error, attempted to set a FhirResource object instances' {Common.Enums.FhirMajorVersion.Stu3.GetLiteral()} property when the {Common.Enums.FhirMajorVersion.R4.GetLiteral()} resource property was already set.";
          throw new Bug.Common.Exceptions.FhirVersionFatalException(message);
        }
        _Stu3 = value;
      }
    }

    private R4Model.Resource _R4;
    public R4Model.Resource R4
    {
      get
      {
        if (this.R4 != null)
        {
          string message = $"Internal server error, attempted to get a FhirResource object instances' {Common.Enums.FhirMajorVersion.R4.GetLiteral()} property when the {Common.Enums.FhirMajorVersion.Stu3.GetLiteral()} resource property was already set.";
          throw new Bug.Common.Exceptions.FhirVersionFatalException(message);
        }
        return _R4;
      }
      set
      {
        if (this.R4 != null)
        {
          string message = $"Internal server error, attempted to set a FhirResource object instances' {Common.Enums.FhirMajorVersion.R4.GetLiteral()} property when the {Common.Enums.FhirMajorVersion.Stu3.GetLiteral()} resource property was already set.";
          throw new Bug.Common.Exceptions.FhirVersionFatalException(message);
        }
        _R4 = value;
      }
    }
  }
}
