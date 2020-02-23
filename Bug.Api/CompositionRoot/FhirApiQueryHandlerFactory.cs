using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Update;
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
    public IQueryHandler<Logic.Query.FhirApi.Create.CreateQuery, FhirApiResult> GetCreateCommand()
    {
      return (IQueryHandler<Logic.Query.FhirApi.Create.CreateQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<Logic.Query.FhirApi.Create.CreateQuery, FhirApiResult>));
    }
  }
}
