using Bug.Common.Enums;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Service.TableService
{
  public class FhirVersionTableService : IFhirVersionTableService
  {
    private readonly IFhirVersionRepository IFhirVersionRepository;
    private readonly IFhirVersionCache IFhirVersionCache;
    public FhirVersionTableService(IFhirVersionRepository IFhirVersionRepository, IFhirVersionCache IFhirVersionCache)
    {
      this.IFhirVersionRepository = IFhirVersionRepository;
      this.IFhirVersionCache = IFhirVersionCache;
    }

    public async Task<FhirVersion> GetSetFhirVersion(FhirMajorVersion fhirMajorVersion)
    {
      FhirVersion? FhirVersion = await IFhirVersionCache.GetAsync(fhirMajorVersion);
      if (FhirVersion is null)
      {
        FhirVersion = new FhirVersion()
        {
          FhirMajorVersion = fhirMajorVersion,
          Code = fhirMajorVersion.GetCode()
        };
        IFhirVersionRepository.Add(FhirVersion);
        await IFhirVersionRepository.SaveChangesAsync();
        await IFhirVersionCache.SetAsync(FhirVersion);
      }
      return FhirVersion;
    }
  }
}
