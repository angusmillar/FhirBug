using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Query.FhirApi;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.CompositionRoot;

namespace Bug.Api.CompositionRoot
{
  public class FhirApiQueryHandlerFactory : IFhirApiQueryHandlerFactory
  {
    private readonly Container _container;
    public FhirApiQueryHandlerFactory(Container container)
    {
      this._container = container;
    }
    public IQueryHandler<Logic.Query.FhirApi.Update.UpdateQuery, FhirApiResult> GetUpdateCommand()
    {
      return (IQueryHandler<Logic.Query.FhirApi.Update.UpdateQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<Logic.Query.FhirApi.Update.UpdateQuery, FhirApiResult>));            
    }
    public IQueryHandler<Logic.Query.FhirApi.Create.CreateQuery, FhirApiResult> GetCreateCommand()
    {
      return (IQueryHandler<Logic.Query.FhirApi.Create.CreateQuery, FhirApiResult>)_container.GetInstance(typeof(IQueryHandler<Logic.Query.FhirApi.Create.CreateQuery, FhirApiResult>));
    }
  }
}
