using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Update;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirApiQueryHandlerFactory
  {
    IQueryHandler<Query.FhirApi.Update.UpdateQuery, FhirApiResult> GetUpdateCommand();
    IQueryHandler<Query.FhirApi.Create.CreateQuery, FhirApiResult> GetCreateCommand();
  }
}