using Microsoft.EntityFrameworkCore;
using Bug.Data.Repository.Base;
using Bug.Logic.DomainModel;
using Bug.Common.Interfaces.Repository;
using System.Threading.Tasks;
using Bug.Common.Interfaces.DomainModel;
using System;
using Bug.Common.DateTimeTools;

namespace Bug.Data.Repository
{
  public class ServiceBaseUrlRepository : Repository<ServiceBaseUrl>, IServiceBaseUrlRepository
  {
    public ServiceBaseUrlRepository(AppDbContext context)
      : base(context) { }

    public async Task<IServiceBaseUrl?> GetBy(Bug.Common.Enums.FhirVersion fhirVersion, string url)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.FhirVersionId == fhirVersion & x.Url == url);
    }

    public async Task<IServiceBaseUrl?> GetPrimary(Bug.Common.Enums.FhirVersion fhirVersion)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.IsPrimary == true & x.FhirVersionId == fhirVersion);
    }

    public async Task<IServiceBaseUrl> AddAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url, bool IsPrimary)
    {
      var Now = DateTimeOffset.Now.ToZulu();
      var ServiceBaseUrl = new ServiceBaseUrl() { FhirVersionId = fhirVersion, Url = url, IsPrimary = IsPrimary, Created = Now, Updated = Now };
      base.Add(ServiceBaseUrl);
      await base.SaveChangesAsync();
      return ServiceBaseUrl;
    }
    public new async Task SaveChangesAsync()
    {
      await base.SaveChangesAsync(); 
    }

  }
}

