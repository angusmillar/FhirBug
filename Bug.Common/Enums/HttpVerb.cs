using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum HttpVerb
  {
    [EnumInfo("POST", "Post")]
    POST = 1,
    [EnumInfo("PUT", "Put")]
    PUT = 2,
    [EnumInfo("GET", "Get")]
    GET = 3,
    [EnumInfo("DELETE", "Delete")]
    DELETE = 4,
    [EnumInfo("HEAD", "Head")]
    HEAD = 5,
    [EnumInfo("PATCH", "Patch")]
    PATCH = 6
  };
}
