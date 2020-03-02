using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.Read;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Query.FhirApi.VRead;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirApiQueryHandlerFactory
  {
    IQueryHandler<UpdateQuery, FhirApiResult> GetUpdateCommand();
    IQueryHandler<CreateQuery, FhirApiResult> GetCreateCommand();
    IQueryHandler<ReadQuery, FhirApiResult> GetReadCommand();
    IQueryHandler<VReadQuery, FhirApiResult> GetVReadCommand();
  }
}