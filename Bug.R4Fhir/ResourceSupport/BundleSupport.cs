using Bug.R4Fhir.Enums;
using Hl7.Fhir.Model;
using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.Bundle;

namespace Bug.R4Fhir.ResourceSupport
{
  public class BundleSupport : IR4BundleSupport
  {
    public Common.FhirTools.FhirResource GetFhirResource(BundleModel bundleModel)
    {
      Bundle Bundle = new Bundle();
      BundleTypeMap BundleTypeMap = new BundleTypeMap();
      Bundle.Type = BundleTypeMap.GetForward(bundleModel.Type);
      Bundle.Timestamp = bundleModel.TimeStamp;
      Bundle.Total = bundleModel.Total;
      if (bundleModel.Link is object)
      {
        Bundle.Link = new List<Bundle.LinkComponent>();
        foreach (var link in bundleModel.Link)
        {
          Bundle.Link.Add(new Bundle.LinkComponent()
          {
            Relation = link.Relation,
            Url = link.Url.OriginalString
          });
        }
      }

      if (bundleModel.Entry is object)
      {
        Bundle.Entry = new List<Bundle.EntryComponent>();
        foreach (var entry in bundleModel.Entry)
        {
          var EntryComponent = new Bundle.EntryComponent();
          Bundle.Entry.Add(EntryComponent);
          if (bundleModel.Link is object)
          {
            EntryComponent.Link = new List<Bundle.LinkComponent>();
            foreach (var link in bundleModel.Link)
            {
              EntryComponent.Link.Add(new Bundle.LinkComponent()
              {
                Relation = link.Relation,
                Url = link.Url.OriginalString
              });
            }
          }

          EntryComponent.FullUrl = entry.FullUrl!.OriginalString;
          if (entry.Resource is object)
          {
            EntryComponent.Resource = entry.Resource.R4;
          }
          if (entry.Search is object)
          {
            SearchEntryModeMap SearchEntryModeMap = new SearchEntryModeMap();
            EntryComponent.Search = new Bundle.SearchComponent();
            if (entry.Search.Mode.HasValue)
            {
              EntryComponent.Search.Mode = SearchEntryModeMap.GetForward(entry.Search.Mode.Value);
            }
            EntryComponent.Search.Score = entry.Search.Score;
          }
          if (entry.Request is object)
          {
            EntryComponent.Request = new Bundle.RequestComponent();
            HttpVerbMap HttpVerbMap = new HttpVerbMap();
            EntryComponent.Request.Method = HttpVerbMap.GetForward(entry.Request.Method);
            if (!string.IsNullOrWhiteSpace(entry.Request.Url))
              EntryComponent.Request.Url = entry.Request.Url;
            EntryComponent.Request.IfNoneMatch = entry.Request.IfNoneMatch;
            EntryComponent.Request.IfModifiedSince = entry.Request.IfModifiedSince;
            EntryComponent.Request.IfMatch = entry.Request.IfMatch;
            EntryComponent.Request.IfNoneExist = entry.Request.IfNoneExist;
          }
          if (entry.Response is object)
          {
            EntryComponent.Response = new Bundle.ResponseComponent();
            EntryComponent.Response.Status = entry.Response.Status;
            if (entry.Response.Location is object)
            {
              EntryComponent.Response.Location = entry.Response.Location.OriginalString;
            }
            EntryComponent.Response.Etag = entry.Response.ETag;
            EntryComponent.Response.LastModified = entry.Response.LastModified;
            if (entry.Response.OutCome is object && entry.Response.OutCome.Stu3 is object)
            {
              EntryComponent.Response.Outcome = entry.Response.OutCome.R4;
            }
          }
        }
      }
      return new Common.FhirTools.FhirResource(Common.Enums.FhirVersion.R4) { R4 = Bundle };
    }
  }
}
