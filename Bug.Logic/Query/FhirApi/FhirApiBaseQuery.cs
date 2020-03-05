using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.UriSupport;
using Microsoft.Extensions.Primitives;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirBaseApiQuery: IQuery<FhirApiResult>, IQuery<ResourceStore>
  {
    public FhirBaseApiQuery(HttpVerb HttpVerb, Common.Enums.FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary)
    {
      this.Method = HttpVerb;
      this.FhirVersion = FhirVersion;
      this.RequestUri = RequestUri;
      this.RequestHeaderDictionary = HeaderDictionary;
    }
    public HttpVerb Method { get; set; }
    public Common.Enums.FhirVersion FhirVersion { get; set; }
    public Uri RequestUri { get; set; }        
    public Dictionary<string, StringValues> RequestHeaderDictionary { get; set; }
  }
}
