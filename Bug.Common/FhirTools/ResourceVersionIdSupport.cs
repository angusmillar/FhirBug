using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.FhirTools
{
  public class ResourceVersionIdSupport : IResourceVersionIdSupport
  {
    public string FirstVersion()
    {
      return "1";
    }
    public string Increment(string VersionNumber, int ValueToAdd = 1)
    {
      int j = AsInterger(VersionNumber);
      return Convert.ToString(j + ValueToAdd);
    }

    public string Decrement(string VersionNumber, int ValueToSubtract = 1)
    {
      int j = AsInterger(VersionNumber);
      if ((j - ValueToSubtract) > -1)
      {
        return Convert.ToString(j - ValueToSubtract);
      }
      else
      {
        throw new ArgumentOutOfRangeException("Resource Version Number was zero and could not be decremented. Value was: " + VersionNumber);
      }
    }

    public int AsInterger(string VersionNumber)
    {
      int j;
      if (Int32.TryParse(VersionNumber, out j))
        return j;
      else
        throw new FormatException("Resource Version Number could not be converted to an int32 integer. Value was: " + VersionNumber);
    }
  }
}
