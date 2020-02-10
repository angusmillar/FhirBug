using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.R4Fhir
{
  public class FhirResourceSupport : IFhirResourceSupport
  {
    public void SetLastUpdated(DateTimeOffset dateTimeOffset, object resource)
    {
      NullCheck(resource, "resource");
      Resource Res = (Resource)resource;
      CreateMeta(Res);
      Res.Meta.LastUpdated = dateTimeOffset;
    }

    public DateTimeOffset? GetLastUpdated( object resource)
    {
      NullCheck(resource, "resource");
      Resource Res = (Resource)resource;      
      return Res?.Meta?.LastUpdated;
    }

    public void SetVersion(string versionId, object resource)
    {
      NullCheck(resource, "resource");
      Resource Res = (Resource)resource;
      CreateMeta(Res);
      Res.Meta.VersionId = versionId;
    }

    public string GetVersion(object resource)
    {
      NullCheck(resource, "resource");
      Resource Res = (Resource)resource;
      return Res?.Meta?.VersionId;
    }

    public void SetSource(Uri uri, object resource)
    {
      NullCheck(resource, "resource");
      NullCheck(uri, "uri");
      Resource Res = (Resource)resource;
      CreateMeta(Res);
      Res.Meta.Source = uri.ToString();
    }

    public void SetProfile(IEnumerable<string> profileList, object resource)
    {
      NullCheck(resource, "resource");
      NullCheck(profileList, "profileList");
      Resource Res = (Resource)resource;
      CreateMeta(Res);
      Res.Meta.Profile = profileList;
    }


    public void SetTag(List<Coding> codingList, object resource)
    {
      NullCheck(resource, "resource");
      NullCheck(codingList, "codingList");
      Resource Res = (Resource)resource;
      CreateMeta(Res);
      Res.Meta.Tag = codingList;
    }

    public void SetSecurity(List<Coding> codingList, object resource)
    {
      NullCheck(resource, "resource");
      NullCheck(codingList, "codingList");
      Resource Res = (Resource)resource;
      CreateMeta(Res);      
      Res.Meta.Security = codingList;
    }

    private void CreateMeta(Resource resource)
    {
      if (resource.Meta == null)
      {
        resource.Meta = new Meta();
      }
    }

    public string GetFhirId(object resource)
    {
      NullCheck(resource, "resource");      
      Resource Res = (Resource)resource;
      return Res.Id;
    }

    public string SetFhirId(string id, object resource)
    {
      NullCheck(resource, "resource");
      Resource Res = (Resource)resource;
      return Res.Id = id;
    }

    private void NullCheck(object instance, string name)
    {
      if (instance == null)
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"{name} parameter can not be null");

    }
  }
}
