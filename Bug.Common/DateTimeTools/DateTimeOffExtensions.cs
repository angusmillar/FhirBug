﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.DateTimeTools
{
  public static class DateTimeOffExtensions
  {
    public static DateTime ToZulu(this DateTimeOffset dateTimeOffset)
    {
      return dateTimeOffset.UtcDateTime;
    }

    public static DateTime NowZulu(this DateTimeOffset dateTimeOffset)
    {
      return DateTimeOffset.Now.UtcDateTime;
    }
  }
}
