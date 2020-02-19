using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api.Extensions
{
  public static class HttpRequestExtension
  {
    public static string GetUrl(this HttpRequest request)
    {
      if (request != null)
        return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
      else
        return string.Empty;

    }
  }
}
