using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.Headers;
using Bug.Logic.Service.ValidatorService;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Read
{
  public class ReadQueryHandler : IQueryHandler<ReadQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IGZipper IGZipper;
    private readonly IServerDateTimeSupport IServerDateTimeSupport;
    private readonly IHeaderService IHeaderService;

    public ReadQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IResourceTypeSupport IResourceTypeSupport,
      IGZipper IGZipper,
      IServerDateTimeSupport IServerDateTimeSupport,
      IHeaderService IHeaderService)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IGZipper = IGZipper;
      this.IServerDateTimeSupport = IServerDateTimeSupport;
      this.IHeaderService = IHeaderService;
    }

    public async Task<FhirApiResult> Handle(ReadQuery query)
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

      ResourceStore? ResourceStore = await IResourceStoreRepository.GetCurrentAsync(query.FhirVersion, ResourceType.Value, query.ResourceId);
      
      if (ResourceStore is object)
      {
        if (ResourceStore.IsDeleted)
        {
          return new FhirApiResult(System.Net.HttpStatusCode.Gone, query.FhirVersion, query.CorrelationId)
          {
            Headers = IHeaderService.GetForRead(IServerDateTimeSupport.ZuluToServerTimeZone(ResourceStore.LastUpdated), ResourceStore.VersionId)
          };
        }
        else
        {
          if (ResourceStore.ResourceBlob is object)
          {
            return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion, query.CorrelationId)
            {
              ResourceId = ResourceStore.ResourceId,
              VersionId = ResourceStore.VersionId,
              FhirResource = IFhirResourceParseJsonService.ParseJson(query.FhirVersion, IGZipper.Decompress(ResourceStore.ResourceBlob)),
              Headers = IHeaderService.GetForRead(IServerDateTimeSupport.ZuluToServerTimeZone(ResourceStore.LastUpdated), ResourceStore.VersionId)
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
