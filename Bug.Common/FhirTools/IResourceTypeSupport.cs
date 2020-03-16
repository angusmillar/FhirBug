using Bug.Common.Enums;

namespace Bug.Common.FhirTools
{
  public interface IResourceTypeSupport
  {
    ResourceType? GetTypeFromName(string name);
  }
}