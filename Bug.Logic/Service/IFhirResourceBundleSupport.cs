﻿using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.Bundle;

namespace Bug.Logic.Service
{
  public interface IFhirResourceBundleSupport
  {
    FhirResource GetFhirResource(FhirMajorVersion fhirMajorVersion, BundleModel bundleModel);
  }
}