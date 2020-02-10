using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Bug.Stu3Fhir
{
  public interface IFhirResourceSupport : IFhirResourceIdSupport
  {
    void SetLastUpdated(DateTimeOffset dateTimeOffset, object resource);
    DateTimeOffset? GetLastUpdated(object resource);
    void SetProfile(IEnumerable<string> profileList, object resource);
    void SetSecurity(List<Coding> codingList, object resource);
    void SetTag(List<Coding> codingList, object resource);
    void SetVersion(string versionId, object resource);
    string GetVersion(object resource);
  }
}