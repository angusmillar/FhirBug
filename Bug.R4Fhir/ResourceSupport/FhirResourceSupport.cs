﻿using Bug.Common.FhirTools;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.R4Fhir.ResourceSupport
{
  public class FhirResourceSupport : IR4FhirResourceIdSupport, IR4FhirResourceVersionSupport, IR4FhirResourceLastUpdatedSupport, IR4FhirResourceNameSupport
  {
    public void SetLastUpdated(DateTimeOffset dateTimeOffset, IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");      
      CreateMeta(fhirResource.R4);
      fhirResource.R4.Meta.LastUpdated = dateTimeOffset;
    }

    public DateTimeOffset? GetLastUpdated(IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");
      return fhirResource.R4?.Meta?.LastUpdated;
    }

    public void SetVersion(string versionId, IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");      
      CreateMeta(fhirResource.R4);
      fhirResource.R4.Meta.VersionId = versionId;
    }

    public string? GetVersion(IFhirResourceR4 fhirResource)
    {
      if (fhirResource is null)
      {
        throw new ArgumentNullException(paramName: nameof(fhirResource));
      }
      if (fhirResource.R4 is null)
      {
        throw new ArgumentNullException(paramName: nameof(fhirResource.R4));
      }

      if (fhirResource.R4?.Meta is null)
      {
        return null;
      }
      else
      {
        return fhirResource.R4?.Meta?.VersionId;
      }      
      
    }

    public void SetSource(Uri uri, IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");
      NullCheck(uri, "uri");      
      CreateMeta(fhirResource.R4);
      fhirResource.R4.Meta.Source = uri.ToString();
    }

    public void SetProfile(IEnumerable<string> profileList, IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");
      NullCheck(profileList, "profileList");      
      CreateMeta(fhirResource.R4);
      fhirResource.R4.Meta.Profile = profileList;
    }


    public void SetTag(List<Coding> codingList, IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");
      NullCheck(codingList, "codingList");      
      CreateMeta(fhirResource.R4);
      fhirResource.R4.Meta.Tag = codingList;
    }

    public void SetSecurity(List<Coding> codingList, IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");
      NullCheck(codingList, "codingList");
      CreateMeta(fhirResource.R4);
      fhirResource.R4.Meta.Security = codingList;
    }

    private void CreateMeta(Resource resource)
    {
      if (resource.Meta == null)
      {
        resource.Meta = new Meta();
      }
    }

    public string? GetFhirId(IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");      
      return fhirResource.R4.Id;
    }

    public string SetFhirId(string id, IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");      
      return fhirResource.R4.Id = id;
    }
    public string GetName(IFhirResourceR4 fhirResource)
    {
      NullCheck(fhirResource.R4, "resource");      
      return fhirResource.R4.ResourceType.GetLiteral();
    }

    private void NullCheck(object instance, string name)
    {
      if (instance == null)
        throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"{name} parameter can not be null");

    }
     
  }
}
