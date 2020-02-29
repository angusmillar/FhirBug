using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum HttpVerb
  {
    [EnumInfo("GET", "Get")]
    GET,
    [EnumInfo("HEAD", "Head")]
    HEAD,
    [EnumInfo("POST", "Post")]
    POST,
    [EnumInfo("PUT", "Put")]
    PUT,
    [EnumInfo("DELETE", "Delete")]
    DELETE,
    [EnumInfo("PATCH", "Patch")]
    PATCH
  };
}
