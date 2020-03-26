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

    public async Task<IServiceBaseUrl?> GetBy(string url)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.Url == url);
    }

    public IServiceBaseUrl Add(string url, bool IsPrimary)
    {
      var Now = DateTimeOffset.Now.ToZulu();
      var ServiceBaseUrl = new ServiceBaseUrl() { Url = url, IsPrimary = IsPrimary, Created = Now, Updated = Now };
      base.Add(ServiceBaseUrl);
      return ServiceBaseUrl;
    }
    public new async Task SaveChangesAsync()
    {
      await base.SaveChangesAsync(); 
    }

  }
}

