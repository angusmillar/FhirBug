using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.Bundle;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class FhirResourceBundleSupport : IFhirResourceBundleSupport
  {
    private readonly IFhirResourceBundleSupportFactory IFhirResourceBundleSupportFactory;
    public FhirResourceBundleSupport(IFhirResourceBundleSupportFactory IFhirResourceBundleSupportFactory)
    {
      this.IFhirResourceBundleSupportFactory = IFhirResourceBundleSupportFactory;
    }

    public FhirResource GetFhirResource(FhirVersion fhirMajorVersion, BundleModel bundleModel)
    {
      switch (fhirMajorVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirResourceBundleSupportFactory.GetStu3();
          return Stu3Tool.GetFhirResource(bundleModel);
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirResourceBundleSupportFactory.GetR4();
          return R4Tool.GetFhirResource(bundleModel);
        default:
          throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }
  }
}
