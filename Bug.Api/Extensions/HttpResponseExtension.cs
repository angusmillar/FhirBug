using Bug.Api.Controllers;
using Bug.Logic.Query.FhirApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api.Extensions
{
  public static class HttpResponseExtension
  {
    public static void AddHeaders(this IHeaderDictionary HeaderDictionary, Dictionary<string, StringValues> Headers)
    {
      if (Headers is null)
        throw new ArgumentNullException(nameof(Headers));
      if (HeaderDictionary is null)
        throw new ArgumentNullException(nameof(HeaderDictionary));

      foreach (var Header in Headers)
        HeaderDictionary.Add(Header.Key, Header.Value);
    }

    public static ActionResult<T> PrepareResponse<T>(this FhirApiResult Result, ControllerBase Cont)
      where T : class
    {
      if (Cont is null)
        throw new ArgumentNullException(nameof(Cont));
      if (Result is null)
        throw new ArgumentNullException(nameof(Result));

      Cont.Response.Headers.AddHeaders(Result.Headers);
      Cont.Response.StatusCode = (int)Result.HttpStatusCode;
      switch (Result.FhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          {
            if (Result.FhirResource is object && Result.FhirResource.Stu3 is object)
            {
              return Cont.StatusCode((int)Result.HttpStatusCode, Result.FhirResource.Stu3);
            }
            else
            {
              return Cont.StatusCode((int)Result.HttpStatusCode);
            }
          }          
        case Common.Enums.FhirVersion.R4:
          {
            if (Result.FhirResource is object && Result.FhirResource.R4 is object)
            {
              return Cont.StatusCode((int)Result.HttpStatusCode, Result.FhirResource.R4);
            }
            else
            {
              return Cont.StatusCode((int)Result.HttpStatusCode);
            }
          }          
        default:
          throw new Bug.Common.Exceptions.FhirVersionFatalException(Result.FhirVersion);          
      }
      
    }
  }
}
