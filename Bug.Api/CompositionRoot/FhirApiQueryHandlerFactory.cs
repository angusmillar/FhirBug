using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.History;
using Bug.Logic.Query.FhirApi.Read;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Query.FhirApi.VRead;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirApiQueryHandlerFactory : IFhirApiQueryHandlerFactory
  {
    private readonly Container _container;
    public FhirApiQueryHandlerFactory(Container container)
    {
      this._container = container;
    }
    public IQueryHandler<UpdateQuery, FhirApiResult> GetUpdateCommand()
    {
      return (IQueryHandler<UpdateQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<UpdateQuery, FhirApiResult>));            
    }
    public IQueryHandler<CreateQuery, FhirApiResult> GetCreateCommand()
    {
      return (IQueryHandler<CreateQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<CreateQuery, FhirApiResult>));
    }

    public IQueryHandler<ReadQuery, FhirApiResult> GetReadCommand()
    {
      return (IQueryHandler<ReadQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<ReadQuery, FhirApiResult>));
    }

    public IQueryHandler<VReadQuery, FhirApiResult> GetVReadCommand()
    {
      return (IQueryHandler<VReadQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<VReadQuery, FhirApiResult>));
    }

    public IQueryHandler<HistoryQuery, FhirApiResult> GetHistoryCommand()
    {
      return (IQueryHandler<HistoryQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<HistoryQuery, FhirApiResult>));
    }
  }
}
