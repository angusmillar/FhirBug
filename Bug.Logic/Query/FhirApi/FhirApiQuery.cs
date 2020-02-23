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
    public FhirApiQuery()
    {
      this.RequestHeaderDictionary = new Dictionary<string, StringValues>();
    }
    public FhirMajorVersion? FhirMajorVersion { get; set; }
    public Uri? RequestUriString { get; set; }        
    public Dictionary<string, StringValues> RequestHeaderDictionary { get; set; }

  }
}
