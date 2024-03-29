﻿using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;

namespace Bug.R4Fhir.Indexing.Setter
{
  public interface IR4StringSetter
  {
    IList<IndexString> Set(ITypedElement typedElement, ResourceType resourceType, int searchParameterId, string searchParameterName);
  }
}