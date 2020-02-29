using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.UriSupport;
using Microsoft.Extensions.Primitives;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirApiQuery: IQuery<FhirApiResult>, IQuery<ResourceStore>
  {
    public FhirApiQuery(HttpVerb HttpVerb, FhirMajorVersion FhirMajorVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary)
    {
      this.HttpVerb = HttpVerb;
      this.FhirMajorVersion = FhirMajorVersion;
      this.RequestUri = RequestUri;
      this.RequestHeaderDictionary = HeaderDictionary;
    }
    public HttpVerb HttpVerb { get; set; }
    public FhirMajorVersion FhirMajorVersion { get; set; }
    public Uri RequestUri { get; set; }        
    public Dictionary<string, StringValues> RequestHeaderDictionary { get; set; }
  }
}
