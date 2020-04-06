using Bug.Common.Compression;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.ValidatorService;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.VRead
{
  public class VReadQueryHandler : IQueryHandler<VReadQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IGZipper IGZipper;


    public VReadQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,                  
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IResourceTypeSupport IResourceTypeSupport,
      IGZipper IGZipper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;            
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IGZipper = IGZipper;
    }

    public async Task<FhirApiResult> Handle(VReadQuery query)
    {

      if (!IValidateQueryService.IsValid(query, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirVersion, query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }
      
      Bug.Common.Enums.ResourceType? ResourceType = IResourceTypeSupport.GetTypeFromName(query.ResourceName);
      if (!ResourceType.HasValue)
        throw new System.ArgumentNullException(nameof(ResourceType));

      ResourceStore? ResourceStore = await IResourceStoreRepository.GetVersionAsync(query.FhirVersion, ResourceType.Value, query.ResourceId, query.VersionId);

      if (ResourceStore is object)
      {
        if (ResourceStore.IsDeleted)
        {
          return new FhirApiResult(System.Net.HttpStatusCode.Gone, query.FhirVersion, query.CorrelationId);
        }
        else
        {
          if (ResourceStore.ResourceBlob is object)
          {
            return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion, query.CorrelationId)
            {
              ResourceId = ResourceStore.ResourceId,
              VersionId = ResourceStore.VersionId,
              FhirResource = IFhirResourceParseJsonService.ParseJson(query.FhirVersion, IGZipper.Decompress(ResourceStore.ResourceBlob))
            };
          }
          else
          {
            string message = $"Internal Server Error: A {nameof(ResourceStore.ResourceBlob)} is null however the {nameof(ResourceStore.IsDeleted)} is set to false. This should not happen.";
            throw new FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
          }
        }
      }
      else
      {
        return new FhirApiResult(System.Net.HttpStatusCode.NotFound, query.FhirVersion, query.CorrelationId);
      }

    }
  }
}
