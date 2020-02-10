using Bug.Logic.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bug.Data
{
  public interface IAppDbContext
  {
    void SaveChanges();
    void Dispose();
    DbSet<T> Set<T>() where T : class;
    EntityEntry Entry(object entity);
    DbSet<ResourceStore> ResourceStore { get; set; }
  }
}
