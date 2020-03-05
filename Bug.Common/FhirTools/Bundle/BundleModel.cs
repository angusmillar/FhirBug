using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Common.FhirTools.Bundle
{
  public class BundleModel
  {    
    public BundleModel(BundleType bundleType)
    {
      this.Type = bundleType;
    }
    public BundleType Type { get; set; }

    /// <summary>
    /// TimeStamp is only found in Fhir Version R4
    /// </summary>
    public DateTimeOffset? TimeStamp { get; set; }
    public int? Total { get; set; }
    public List<LinkComponent>? Link { get; set; }
    public List<EntryComponent>? Entry { get; set; }

    public class LinkComponent
    {
      public LinkComponent(string relation, Uri Url)
      {
        this.Relation = Relation;
        this.Url = Url;
      }
      public string Relation { get; set; }
      public Uri Url { get; set; }
    }

    public class EntryComponent
    {
      public List<LinkComponent>? Link { get; set; }
      public Uri? FullUrl { get; set; }
      public FhirResource? Resource { get; set; }
      public SearchComponent? Search { get; set; }
      public RequestComponent? Request { get; set; }
      public ResponseComponent? Response { get; set; }

      public class SearchComponent
      {
        public SearchEntryMode? Mode { get; set; }
        public decimal? Score { get; set; }
      }
    }

    public class RequestComponent
    {
      public RequestComponent(HttpVerb Method, string Url)
      {
        this.Method = Method;
        this.Url = Url;
      }
      public HttpVerb Method { get; set; }
      public string Url { get; set; }
      public string? IfNoneMatch { get; set; }
      public DateTimeOffset? IfModifiedSince { get; set; }
      public string? IfMatch { get; set; }
      public string? IfNoneExist { get; set; }
    }

    public class ResponseComponent
    {
      public ResponseComponent(string Status)
      {
        this.Status = Status;
      }
      public string? Status { get; set; }
      public Uri? Location { get; set; }
      public string? ETag { get; set; }
      public DateTimeOffset? LastModified { get; set; }
      public FhirResource? OutCome { get; set; }
    }
  }
}
