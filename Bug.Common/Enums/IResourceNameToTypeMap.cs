namespace Bug.Common.Enums
{
  public interface IResourceNameToTypeMap
  {
    ResourceType? GetResourceType(string resourceName);
  }
}