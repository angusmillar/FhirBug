using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public class ResourceNameToTypeMap : IResourceNameToTypeMap
  {
    private readonly Dictionary<string, ResourceType> Map;
    public ResourceNameToTypeMap()
    {
      Map = StringToEnumMap<ResourceType>.GetDictionary();
    }

    public ResourceType? GetResourceType(string resourceName)
    {
      if (Map.ContainsKey(resourceName))
      {
        return Map[resourceName];
      }
      else
      {
        return null;
      }
    }
  }
}
