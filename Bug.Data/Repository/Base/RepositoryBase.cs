using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Data.Repository.Base
{
  public class Repository<TEntity> : IRepository<TEntity> 
    where TEntity : BaseDateStamp
  {
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(AppDbContext context)
    {
      _context = context;
      DbSet = _context.Set<TEntity>();
    }

    public async Task SaveChangesAsync()
    {
      await _context.SaveChangesAsync();
    }

    public void Add(TEntity entity)
    {
      DbSet.Add(entity);
    }

    public void Remove(TEntity entity)
    {
      TEntity existing = DbSet.Find(entity);
      if (existing != null) DbSet.Remove(existing);
    }

    public void Remove(int Id)
    {
      DbSet.Remove(DbSet.Find(Id));
    }

    public IEnumerable<TEntity> Get()
    {
      return DbSet.AsEnumerable<TEntity>();
    }

    public IEnumerable<TEntity> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
    {
      return DbSet.Where(predicate).AsEnumerable<TEntity>();
    }

    public void Update(TEntity entity)
    {

      DbSet.Update(entity);

      //_context.Entry(entity).State = EntityState.Modified;      
      //DbSet.Attach(entity);      
    }

    public void Dispose()
    {
      _context.Dispose();
      GC.SuppressFinalize(this);
    }

  }
}
