extern alias Stu3;
extern alias R4;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using R4Model = R4.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

#nullable disable 
namespace Bug.Common.FhirTools
{
  public class FhirResource : IFhirResourceStu3, IFhirResourceR4
  {
    public FhirResource(FhirVersion FhirVersion)
    {
      _FhirVersion = FhirVersion;
    }
    private FhirVersion _FhirVersion;
    public FhirVersion FhirVersion 
    { 
      get
      {
        return _FhirVersion;
      }        
    }

    private Stu3Model.Resource _Stu3;
    public Stu3Model.Resource Stu3
    {
      get
      {
        if (this._FhirVersion == FhirVersion.R4)
        {
          string message = $"Internal server error, attempted to Get a {FhirVersion.Stu3.GetCode()} Resource from a {this.GetType().Name} object instance when the the class was instantated as a FHIR {FhirVersion.R4.GetCode()} version instance.";
          throw new Bug.Common.Exceptions.FhirVersionFatalException(message);
        }
        return _Stu3;
      }
      set
      {
        if (this._FhirVersion == FhirVersion.R4)
        {
          string message = $"Internal server error, attempted to Set a {this.GetType().Name} object instances with a {FhirVersion.Stu3.GetCode()} Resource when the the class was instantated as a FHIR {FhirVersion.R4.GetCode()} version instance.";
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
        if (FhirVersion == FhirVersion.Stu3)
        {
          string message = $"Internal server error, attempted to Get a {FhirVersion.R4.GetCode()} Resource from a {this.GetType().Name} object instance when the the class was instantated as a FHIR {FhirVersion.Stu3.GetCode()} version instance.";
          throw new Bug.Common.Exceptions.FhirVersionFatalException(message);
        }
        return _R4;
      }
      set
      {
        if (FhirVersion == FhirVersion.Stu3)
        {
          string message = $"Internal server error, attempted to Set a {this.GetType().Name} object instances with a {FhirVersion.R4.GetCode()} Resource when the the class was instantated as a FHIR {FhirVersion.Stu3.GetCode()} version instance.";
          throw new Bug.Common.Exceptions.FhirVersionFatalException(message);
        }
        _R4 = value;
      }
    }
  }
}
