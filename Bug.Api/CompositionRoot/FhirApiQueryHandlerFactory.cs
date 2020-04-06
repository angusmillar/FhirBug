using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.Delete;
using Bug.Logic.Query.FhirApi.HistoryBase;
using Bug.Logic.Query.FhirApi.HistoryInstance;
using Bug.Logic.Query.FhirApi.HistoryResource;
using Bug.Logic.Query.FhirApi.Read;
using Bug.Logic.Query.FhirApi.Search;
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

    //FhirApiTransactionalResult
    public IQueryHandler<UpdateQuery, FhirApiTransactionalResult> GetUpdateCommand()
    {
      return (IQueryHandler<UpdateQuery, FhirApiTransactionalResult>)_container.GetInstance(typeof(IQueryHandler<UpdateQuery, FhirApiTransactionalResult>));            
    }
    public IQueryHandler<CreateQuery, FhirApiTransactionalResult> GetCreateCommand()
    {
      return (IQueryHandler<CreateQuery, FhirApiTransactionalResult>)_container.GetInstance(typeof(IQueryHandler<CreateQuery, FhirApiTransactionalResult>));
    }
    public IQueryHandler<DeleteQuery, FhirApiTransactionalResult> GetDeleteCommand()
    {
      return (IQueryHandler<DeleteQuery, FhirApiTransactionalResult>)_container.GetInstance(typeof(IQueryHandler<DeleteQuery, FhirApiTransactionalResult>));
    }

    //FhirApiResult
    public IQueryHandler<ReadQuery, FhirApiResult> GetReadCommand()
    {
      return (IQueryHandler<ReadQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<ReadQuery, FhirApiResult>));
    }

    public IQueryHandler<VReadQuery, FhirApiResult> GetVReadCommand()
    {
      return (IQueryHandler<VReadQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<VReadQuery, FhirApiResult>));
    }

    public IQueryHandler<HistoryInstanceQuery, FhirApiResult> GetHistoryInstanceCommand()
    {
      return (IQueryHandler<HistoryInstanceQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<HistoryInstanceQuery, FhirApiResult>));
    }
    public IQueryHandler<HistoryResourceQuery, FhirApiResult> GetHistoryResourceCommand()
    {
      return (IQueryHandler<HistoryResourceQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<HistoryResourceQuery, FhirApiResult>));
    }
    public IQueryHandler<HistoryBaseQuery, FhirApiResult> GetHistoryBaseCommand()
    {
      return (IQueryHandler<HistoryBaseQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<HistoryBaseQuery, FhirApiResult>));
    }
    public IQueryHandler<SearchQuery, FhirApiResult> GetSearchCommand()
    {
      return (IQueryHandler<SearchQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<SearchQuery, FhirApiResult>));
    }



  }
}
