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
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;

namespace Bug.Logic.Query.FhirApi.Update.Decorator
{
  public class UpdateDataCollectionQueryDecorator<TQuery, TResult> 
    : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>    
  {
    private readonly IQueryHandler<TQuery, TResult> Decorated;    
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory;
    private readonly IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory;
    private readonly IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory;
    private readonly IResourceVersionIdSupport IResourceVersionIdSupport;


    public UpdateDataCollectionQueryDecorator(IQueryHandler<TQuery, TResult> decorated,
      IResourceStoreRepository IResourceStoreRepository,
      IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory,
      IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory,
      IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory,
      IResourceVersionIdSupport IResourceVersionIdSupport
      )
    {
      this.Decorated = decorated;      
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceIdSupportFactory = IFhirResourceIdSupportFactory;
      this.IFhirResourceVersionSupportFactory = IFhirResourceVersionSupportFactory;
      this.IFhirResourceLastUpdatedSupportFactory = IFhirResourceLastUpdatedSupportFactory;
      this.IResourceVersionIdSupport = IResourceVersionIdSupport;
    }

    public async Task<TResult> Handle(TQuery command)
    {
      //if (command.FhirMajorVersion == FhirMajorVersion.Stu3)
      //{
      //  //LastUpdated
      //  command.LastUpdated = DateTimeOffset.Now;
      //  var FhirResourceLastUpdatedSupport = IFhirResourceLastUpdatedSupportFactory.GetStu3();        
      //  FhirResourceLastUpdatedSupport.SetLastUpdated(command.LastUpdated, command.Resource);

      //  //Resource Id
      //  var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();
      //  command.FhirId = FhirResourceIdSupport.GetFhirId(command.Resource);
      //  var FhirResourceVersionSupport = IFhirResourceVersionSupportFactory.GetStu3();
      //  ResourceStore ResourceStore = await IResourceStoreRepository.GetByFhirIdAsync(command.FhirId);
      //  if (ResourceStore != null)
      //  {
      //    command.VersionId = IResourceVersionIdSupport.Increment(ResourceStore.VersionId);
      //  }
      //  else
      //  {
      //    command.VersionId = IResourceVersionIdSupport.FirstVersion();
      //  }
      //  FhirResourceVersionSupport.SetVersion(command.VersionId, command.Resource);     
      //}
      //else if (command.FhirMajorVersion == FhirMajorVersion.R4)
      //{
      //  //LastUpdated
      //  command.LastUpdated = DateTimeOffset.Now;
      //  var FhirResourceLastUpdatedSupport = IFhirResourceLastUpdatedSupportFactory.GetR4();
      //  FhirResourceLastUpdatedSupport.SetLastUpdated(command.LastUpdated, command.Resource);

      //  //Resource Id
      //  var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetR4();
      //  command.FhirId = FhirResourceIdSupport.GetFhirId(command.Resource);
      //  var FhirResourceVersionSupport = IFhirResourceVersionSupportFactory.GetR4();
      //  ResourceStore ResourceStore = await IResourceStoreRepository.GetByFhirIdAsync(command.FhirId);
      //  if (ResourceStore != null)
      //  {
      //    command.VersionId = ResourceStore.VersionId; //Add one
      //  }
      //  else
      //  {
      //    command.VersionId = "1";
      //  }
      //  FhirResourceVersionSupport.SetVersion(command.VersionId, command.Resource);
      //}
      //else
      //{
      //  throw new FhirVersionFatalException(command.FhirMajorVersion);
      //}

      return await this.Decorated.Handle(command);
    }
  }
}
