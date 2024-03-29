﻿using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Fhir
{
  public class FhirResourceVersionSupport : IFhirResourceVersionSupport
  {
    private readonly IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory;
    public FhirResourceVersionSupport(IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory)
    {
      this.IFhirResourceVersionSupportFactory = IFhirResourceVersionSupportFactory;
    }

    public string? GetVersion(FhirResource fhirResource)
    {
      switch (fhirResource.FhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirResourceVersionSupportFactory.GetStu3();
          return Stu3Tool.GetVersion(fhirResource);
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirResourceVersionSupportFactory.GetR4();
          return R4Tool.GetVersion(fhirResource);
        default:
          throw new FhirVersionFatalException(fhirResource.FhirVersion);
      }
    }

    public void SetVersion(FhirResource fhirResource, int versionId)
    {
      switch (fhirResource.FhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirResourceVersionSupportFactory.GetStu3();
          Stu3Tool.SetVersion(versionId.ToString(), fhirResource);
          break;
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirResourceVersionSupportFactory.GetR4();
          R4Tool.SetVersion(versionId.ToString(), fhirResource);
          break;
        default:
          throw new FhirVersionFatalException(fhirResource.FhirVersion);
      }
    }
  }
}
