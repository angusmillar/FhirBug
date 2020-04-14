using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.R4Fhir.Serialization;
using Bug.Stu3Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Fhir
{
  public class FhirResourceJsonSerializationService : IFhirResourceJsonSerializationService
  {
    private readonly IStu3SerializationToJsonBytes IStu3SerializationToJsonBytes;
    private readonly IR4SerializationToJsonBytes IR4SerializationToJsonBytes;
    public FhirResourceJsonSerializationService(IStu3SerializationToJsonBytes IStu3SerializationToJsonBytes,
      IR4SerializationToJsonBytes IR4SerializationToJsonBytes)
    {
      this.IStu3SerializationToJsonBytes = IStu3SerializationToJsonBytes;
      this.IR4SerializationToJsonBytes = IR4SerializationToJsonBytes;
    }

    public byte[] SerializeToJsonBytes(Common.FhirTools.FhirResource fhirResource)
    {
      return fhirResource.FhirVersion switch
      {
        Common.Enums.FhirVersion.Stu3 => IStu3SerializationToJsonBytes.SerializeToJsonBytes(fhirResource),
        Common.Enums.FhirVersion.R4 => IR4SerializationToJsonBytes.SerializeToJsonBytes(fhirResource),
        _ => throw new FhirVersionFatalException(fhirResource.FhirVersion),
      };
    }

    public byte[] SerializeToJsonBytes(Common.FhirTools.FhirContainedResource FhirContainedResource)
    {
      return FhirContainedResource.FhirVersion switch
      {
        Common.Enums.FhirVersion.Stu3 => IStu3SerializationToJsonBytes.SerializeToJsonBytes(FhirContainedResource),
        Common.Enums.FhirVersion.R4 => IR4SerializationToJsonBytes.SerializeToJsonBytes(FhirContainedResource),
        _ => throw new FhirVersionFatalException(FhirContainedResource.FhirVersion),
      };
    }
  }
}
