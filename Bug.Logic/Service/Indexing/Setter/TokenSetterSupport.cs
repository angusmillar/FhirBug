﻿using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Hl7.Fhir.ElementModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Indexing.Setter
{
  public class TokenSetterSupport : ITokenSetterSupport
  {
    private readonly IFhirIndexTokenSetterSupportFactory IFhirIndexTokenSetterSupportFactory;
    public TokenSetterSupport(IFhirIndexTokenSetterSupportFactory IFhirIndexTokenSetterSupportFactory)
    {
      this.IFhirIndexTokenSetterSupportFactory = IFhirIndexTokenSetterSupportFactory;
    }

    public IList<Bug.Common.Dto.Indexing.IndexToken> Set(Bug.Common.Enums.FhirVersion fhirVersion, ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      switch (fhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          var Stu3Tool = IFhirIndexTokenSetterSupportFactory.GetStu3();
          return Stu3Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        case Common.Enums.FhirVersion.R4:
          var R4Tool = IFhirIndexTokenSetterSupportFactory.GetR4();
          return R4Tool.Set(typedElement, resourceType, searchParameterId, searchParameterName);
        default:
          throw new FhirVersionFatalException(fhirVersion);
      }
    }
  }
}
