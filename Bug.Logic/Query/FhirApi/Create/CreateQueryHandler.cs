using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.FhirTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.TableService;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Create
{
  public class CreateQueryHandler : IQueryHandler<CreateQuery, FhirApiResult>
  {
    
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IResourceNameTableService IResourceNameTableService;
    private readonly IMethodTableService IMethodTableService;
    private readonly IFhirVersionTableService IFhirVersionTableService;
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IGZipper IGZipper;        

    public CreateQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IResourceNameTableService IResourceNameTableService,
      IFhirVersionTableService IFhirVersionTableService,
      IMethodTableService IMethodTableService,      
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,     
      IUpdateResourceService IUpdateResourceService,
      IGZipper IGZipper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IResourceNameTableService = IResourceNameTableService;
      this.IFhirVersionTableService = IFhirVersionTableService;
      this.IMethodTableService = IMethodTableService;
      this.IFhirResourceJsonSerializationService = IFhirResourceJsonSerializationService;      
      this.IUpdateResourceService = IUpdateResourceService;
      this.IGZipper = IGZipper;            
    }

    public async Task<FhirApiResult> Handle(CreateQuery query)
    {
      if (query.ResourceName is null)
        throw new ArgumentNullException(paramName: nameof(query.ResourceName));
      
      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirMajorVersion)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      var UpdateResource = new UpdateResource(query.FhirResource)
      {
        ResourceId = FhirGuidSupport.NewFhirGuid(),
        VersionId = 1,
        LastUpdated = DateTimeOffset.Now.ToZulu()
      };

      FhirResource UpdatedFhirResource = IUpdateResourceService.Process(UpdateResource);
      byte[] ResourceBytes = IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource);

      ResourceName ResourceName = await IResourceNameTableService.GetSetResourceName(query.ResourceName);
      FhirVersion FhirVersion = await IFhirVersionTableService.GetSetFhirVersion(UpdatedFhirResource.FhirMajorVersion);
      Method Method = await IMethodTableService.GetSetMethod(query.Method);

      var ResourceStore = new ResourceStore()
      {
        ResourceId = UpdateResource.ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = UpdateResource.VersionId.Value,
        LastUpdated = UpdateResource.LastUpdated.Value,
        ResourceBlob = IGZipper.Compress(ResourceBytes),        
        FkResourceNameId = ResourceName.Id,
        FkFhirVersionId = FhirVersion.Id,
        FkMethodId = Method.Id
      };     
      

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      var OutCome = new FhirApiResult(System.Net.HttpStatusCode.Created, query.FhirMajorVersion)
      {
        ResourceId = UpdateResource.ResourceId,
        FhirResource = UpdatedFhirResource,
        VersionId = UpdateResource.VersionId
      };

      return OutCome;
    }
  }
}
