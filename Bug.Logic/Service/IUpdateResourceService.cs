using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IUpdateResourceService
  {
    FhirResource Process(UpdateResource UpdateResource);
  }
}