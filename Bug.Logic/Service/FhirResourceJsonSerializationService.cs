using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.R4Fhir.Serialization;
using Bug.Stu3Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
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

    public byte[] SerializeToJsonBytes(FhirResource fhirResource)
    {
      switch (fhirResource.FhirMajorVersion)
      {
        case FhirMajorVersion.Stu3:
          return IStu3SerializationToJsonBytes.SerializeToJsonBytes(fhirResource);
        case FhirMajorVersion.R4:
          return IR4SerializationToJsonBytes.SerializeToJsonBytes(fhirResource);
        default:
          throw new FhirVersionFatalException(fhirResource.FhirMajorVersion);
      }
    }
  }
}
