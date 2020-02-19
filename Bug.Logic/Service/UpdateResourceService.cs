using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Interfaces.Repository;
//using Bug.Logic..FhirApi.Create;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Common.DateTimeTools;

namespace Bug.Logic.Service
{
  public class UpdateResourceService
  {
    private readonly IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory;
    private readonly IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory;
    private readonly IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory;
   
    private readonly IResourceStoreRepository IResourceStoreRepository;
    public UpdateResourceService(
      IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory,
      IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory,
      IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory,      
      IResourceStoreRepository IResourceStoreRepository)
    {
      this.IFhirResourceIdSupportFactory = IFhirResourceIdSupportFactory;
      this.IFhirResourceVersionSupportFactory = IFhirResourceVersionSupportFactory;
      this.IFhirResourceLastUpdatedSupportFactory = IFhirResourceLastUpdatedSupportFactory;      
      this.IResourceStoreRepository = IResourceStoreRepository;
    }

    public async Task<bool> Process(UpdateResource UpdateResource)
    {
      
      if (UpdateResource.FhirMajorVersion == FhirMajorVersion.Stu3)
      {
        //LastUpdated
        UpdateResource.LastUpdated = DateTimeOffset.Now.ToZulu();
        var FhirResourceLastUpdatedSupport = IFhirResourceLastUpdatedSupportFactory.GetStu3();
        FhirResourceLastUpdatedSupport.SetLastUpdated(UpdateResource.LastUpdated, UpdateResource.Resource);

        //Resource Id        
        var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();        
        UpdateResource.ResourceId = FhirResourceIdSupport.GetFhirId(UpdateResource.Resource);

        UpdateResource.VersionId = Bug.Common.FhirTools.FhirGuidSupport.NewFhirGuid();
        var FhirResourceVersionSupport = IFhirResourceVersionSupportFactory.GetStu3();
        FhirResourceVersionSupport.SetVersion(UpdateResource.VersionId, UpdateResource.Resource);        
      }
      else if (UpdateResource.FhirMajorVersion == FhirMajorVersion.R4)
      {
        //LastUpdated
        UpdateResource.LastUpdated = DateTimeOffset.Now.ToZulu();
        var FhirResourceLastUpdatedSupport = IFhirResourceLastUpdatedSupportFactory.GetR4();
        FhirResourceLastUpdatedSupport.SetLastUpdated(UpdateResource.LastUpdated, UpdateResource.Resource);

        //Resource Id        
        var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetR4();
        UpdateResource.ResourceId = FhirResourceIdSupport.GetFhirId(UpdateResource.Resource);

        UpdateResource.VersionId = Bug.Common.FhirTools.FhirGuidSupport.NewFhirGuid();
        var FhirResourceVersionSupport = IFhirResourceVersionSupportFactory.GetR4();
        FhirResourceVersionSupport.SetVersion(UpdateResource.VersionId, UpdateResource.Resource);
      }
      else
      {
        throw new FhirVersionFatalException(UpdateResource.FhirMajorVersion);
      }
      return true;
    }

  }
}
