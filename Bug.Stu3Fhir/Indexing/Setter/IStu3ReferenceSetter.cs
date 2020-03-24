﻿using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Hl7.Fhir.ElementModel;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.Indexing.Setter
{
  public interface IStu3ReferenceSetter
  {
    IList<IndexReference> Set(ITypedElement typedElement, ResourceType resourceType, int searchParameterId, string searchParameterName);
  }
}