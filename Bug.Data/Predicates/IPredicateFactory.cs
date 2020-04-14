﻿using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using LinqKit;
using System.Collections.Generic;

namespace Bug.Data.Predicates
{
  public interface IPredicateFactory
  {
    ExpressionStarter<ResourceStore> Get(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, IList<ISearchQueryBase> SearchQueryList);
  }
}