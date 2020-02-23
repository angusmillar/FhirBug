using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.Repository;

namespace Bug.Logic.Service
{
  public class UpdateResourceService : IUpdateResourceService
  {
    private readonly IFhirResourceIdSupport IFhirResourceIdSupport;
    private readonly IFhirResourceVersionSupport IFhirResourceVersionSupport;
    private readonly IFhirResourceLastUpdatedSupport IFhirResourceLastUpdatedSupport;
    
    public UpdateResourceService(
      IFhirResourceIdSupport IFhirResourceIdSupport,
      IFhirResourceVersionSupport IFhirResourceVersionSupport,
      IFhirResourceLastUpdatedSupport IFhirResourceLastUpdatedSupport)
    {
      this.IFhirResourceIdSupport = IFhirResourceIdSupport;
      this.IFhirResourceVersionSupport = IFhirResourceVersionSupport;
      this.IFhirResourceLastUpdatedSupport = IFhirResourceLastUpdatedSupport;
    }

    public FhirResource Process(UpdateResource UpdateResource)
    {
      if (UpdateResource == null)
        throw new System.NullReferenceException();

      if (UpdateResource.FhirResource == null)
        throw new System.NullReferenceException();

      if (!string.IsNullOrWhiteSpace(UpdateResource.ResourceId))
      {
        IFhirResourceIdSupport.SetFhirId(UpdateResource.FhirResource, UpdateResource.ResourceId);
      }

      if (!string.IsNullOrWhiteSpace(UpdateResource.VersionId))
      {
        IFhirResourceVersionSupport.SetVersion(UpdateResource.FhirResource, UpdateResource.VersionId);
      }
      
      if (UpdateResource.LastUpdated.HasValue)
      {
        IFhirResourceLastUpdatedSupport.SetLastUpdated(UpdateResource.FhirResource, UpdateResource.LastUpdated.Value);
      }
      
      return UpdateResource.FhirResource;
    }

  }
}
