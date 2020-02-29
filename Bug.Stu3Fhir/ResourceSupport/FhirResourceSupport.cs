using Bug.Common.FhirTools;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public class FhirResourceSupport : IStu3FhirResourceIdSupport, IStu3FhirResourceVersionSupport, IStu3FhirResourceLastUpdatedSupport, IStu3FhirResourceNameSupport
  {
    public void SetLastUpdated(DateTimeOffset dateTimeOffset, IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      CreateMeta(fhirResource.Stu3);
      fhirResource.Stu3.Meta.LastUpdated = dateTimeOffset;
    }

    public DateTimeOffset? GetLastUpdated(IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      return fhirResource.Stu3?.Meta?.LastUpdated;
    }

    public void SetVersion(string versionId, IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      CreateMeta(fhirResource.Stu3);
      fhirResource.Stu3.Meta.VersionId = versionId;
    }

    public string? GetVersion(IFhirResourceStu3 fhirResource)
    {
      if (fhirResource is null)
      {
        throw new ArgumentNullException(paramName: nameof(fhirResource));
      }
      if (fhirResource.Stu3 is null)
      {
        throw new ArgumentNullException(paramName: nameof(fhirResource.Stu3));
      }

      if (fhirResource.Stu3?.Meta is null)
      {
        return null;
      }
      else
      {
        return fhirResource.Stu3?.Meta?.VersionId;
      }

    }

    public void SetProfile(IEnumerable<string> profileList, IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      NullCheck(profileList, "profileList");
      CreateMeta(fhirResource.Stu3);
      fhirResource.Stu3.Meta.Profile = profileList;
    }


    public void SetTag(List<Coding> codingList, IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      NullCheck(codingList, "codingList");
      CreateMeta(fhirResource.Stu3);
      fhirResource.Stu3.Meta.Tag = codingList;
    }

    public void SetSecurity(List<Coding> codingList, IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      NullCheck(codingList, "codingList");
      CreateMeta(fhirResource.Stu3);
      fhirResource.Stu3.Meta.Security = codingList;
    }

    private void CreateMeta(Resource resource)
    {
      if (resource.Meta == null)
      {
        resource.Meta = new Meta();
      }
    }

    public string GetFhirId(IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      return fhirResource.Stu3.Id;
    }

    public void SetFhirId(string id, IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      fhirResource.Stu3.Id = id;
    }
    public string GetName(IFhirResourceStu3 fhirResource)
    {
      NullCheck(fhirResource.Stu3, "resource");
      return fhirResource.Stu3.ResourceType.GetLiteral();
    }

    private void NullCheck(object instance, string name)
    {
      if (instance == null)
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"{name} parameter can not be null");

    }

  }
}
