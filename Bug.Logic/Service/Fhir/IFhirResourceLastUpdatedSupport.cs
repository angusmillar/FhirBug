﻿using Bug.Common.FhirTools;
using System;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceLastUpdatedSupport
  {
    DateTimeOffset? GetLastUpdated(FhirResource fhirResource);
    void SetLastUpdated(FhirResource fhirResource, DateTimeOffset dateTime);
  }
}