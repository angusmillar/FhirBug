using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Query;
using Bug.Common.Enums;
using Bug.Logic.Query.FhirApi;
using Microsoft.Extensions.Logging;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.DomainModel;

namespace Bug.Logic.Query.FhirApi.Update.Decorator
{
  public class UpdateValidatorQueryDecorator<TQuery, TResult> 
    : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>    
  {
    private readonly IQueryHandler<TQuery, TResult> Decorated;
    private readonly ILogger ILogger;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory;


    public UpdateValidatorQueryDecorator(IQueryHandler<TQuery, TResult> decorated, ILogger logger, IResourceStoreRepository IResourceStoreRepository, IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory)
    {
      this.Decorated = decorated;
      this.ILogger = logger;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceIdSupportFactory = IFhirResourceIdSupportFactory;
    }

    public async Task<TResult> Handle(TQuery command)
    {
      //if (command.FhirMajorVersion == FhirMajorVersion.Stu3)
      //{
      //  var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();
      //  command.FhirId = FhirResourceIdSupport.GetFhirId(command.Resource);
      //  ResourceStore resourceStore = await IResourceStoreRepository.GetByFhirIdAsync(command.FhirId);

      //}

      return await this.Decorated.Handle(command);
    }
  }
}
