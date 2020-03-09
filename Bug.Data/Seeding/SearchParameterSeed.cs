using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Data.Seeding
{
  public static class SearchParameterSeed
  {
    public static SearchParameter[] GetSeedData(DateTime dateTimeNow)
    {
      var List = new List<SearchParameter>()
      {
        new SearchParameter() { Name = "_id", Description = "Logical id of this artifact", FkSearchParamTypeId = Common.Enums.SearchParamType.Token, Url = "http://hl7.org/fhir/SearchParameter/Resource-id", FhirPath = "Resource.id", FkFhirVersionId = Common.Enums.FhirVersion.R4, Created = dateTimeNow, Updated = dateTimeNow },
      };

      return List.ToArray();

    }
  }
}
