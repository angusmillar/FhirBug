using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api.Extensions
{
  public static class HttpRequestExtension
  {    
    public static Uri GetUrl(this HttpRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(paramName: nameof(request));

      return new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}");
      
    }
  }
}
