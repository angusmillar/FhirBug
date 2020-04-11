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
  public class FhirContainedResource : FhirResource
  {
    public FhirContainedResource(Enums.FhirVersion FhirVersion, Enums.ResourceType ResourceType, string ResourceId)      
      :base(FhirVersion)
    {
      this.ResourceId = ResourceId;      
      this.ResourceType = ResourceType;
    }

    public string ResourceId { get; private set; }
    public Enums.ResourceType ResourceType { get; private set; }
    public FhirResource FhirResource { get; private set; }

  }
}
