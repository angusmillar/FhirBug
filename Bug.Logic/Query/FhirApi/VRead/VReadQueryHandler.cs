using Bug.Common.Compression;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.TableService;
using Bug.Logic.Service.ValidatorService;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.VRead
{
  public class VReadQueryHandler : IQueryHandler<VReadQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IGZipper IGZipper;


    public VReadQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,                  
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IGZipper IGZipper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;            
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IGZipper = IGZipper;
    }

    public async Task<FhirApiResult> Handle(VReadQuery query)
    {

      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirMajorVersion)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }      

      ResourceStore? ResourceStore = await IResourceStoreRepository.GetVersionAsync(query.FhirVersion, query.ResourceName, query.ResourceId, query.VersionId);

      if (ResourceStore is object)
      {
        if (ResourceStore.IsDeleted)
        {
          return new FhirApiResult(System.Net.HttpStatusCode.Gone, query.FhirVersion);
        }
        else
        {
          if (ResourceStore.ResourceBlob is object)
          {
            return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion)
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
        return new FhirApiResult(System.Net.HttpStatusCode.NotFound, query.FhirVersion);
      }

    }
  }
}
