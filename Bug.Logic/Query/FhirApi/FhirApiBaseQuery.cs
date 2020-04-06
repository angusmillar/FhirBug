using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.Tools;
using Bug.Logic.UriSupport;
using Microsoft.Extensions.Primitives;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirBaseApiQuery: IQuery<FhirApiResult>, IQuery<FhirApiTransactionalResult>
  {
    public FhirBaseApiQuery(HttpVerb HttpVerb, Common.Enums.FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> RequestQuery, Dictionary<string, StringValues> HeaderDictionary)
    {
      this.Method = HttpVerb;
      this.FhirVersion = FhirVersion;
      this.RequestUri = RequestUri;
      this.RequestHeaderDictionary = HeaderDictionary;
      this.RequestQuery = new FhirSearchQuery();
      this.RequestQuery.Parse(RequestQuery);
      this.CorrelationId = Guid.NewGuid().ToString();
      
    }
    public HttpVerb Method { get; set; }
    public Common.Enums.FhirVersion FhirVersion { get; set; }
    public Uri RequestUri { get; set; }        
    public Dictionary<string, StringValues> RequestHeaderDictionary { get; set; }
    public IFhirSearchQuery RequestQuery { get; set; }
    
    public string CorrelationId { get; set; }
  }
}
