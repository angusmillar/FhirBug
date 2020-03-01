using Bug.Logic.Query;
using Bug.Common.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Interfaces.Repository;
using Bug.Stu3Fhir.Serialization;
using Bug.R4Fhir.Serialization;
using Bug.Common.Compression;
using Bug.Common.Exceptions;
using Bug.Logic.DomainModel;
using Bug.Logic.Service;
using Bug.Common.DateTimeTools;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Caching.Distributed;
using Bug.Logic.CacheService;
using Bug.Logic.Service.TableService;

namespace Bug.Logic.Query.FhirApi.Read
{
  public class ReadQueryHandler : IQueryHandler<ReadQuery, FhirApiResult>
  {    
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IMethodTableService IMethodTableService;
    private readonly IFhirVersionTableService IFhirVersionTableService;

    public ReadQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IResourceNameTableService IResourceNameTableService,
      IFhirVersionTableService IFhirVersionTableService,
      IMethodTableService IMethodTableService)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;      
      this.IFhirVersionTableService = IFhirVersionTableService;
      this.IMethodTableService = IMethodTableService;
    }

    public async Task<FhirApiResult> Handle(ReadQuery query)
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

      Method Method = await IMethodTableService.GetSetMethod(query.Method);

      

      var OutCome = new FhirApiResult(System.Net.HttpStatusCode.Created, query.FhirMajorVersion)
      {
        ResourceId = "1",
        FhirResource = new FhirResource(FhirMajorVersion.Stu3),
        VersionId = 1
      };

      return OutCome;
    }
  }
}
