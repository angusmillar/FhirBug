using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IRepository<TEntity>
  {
    Task SaveChangesAsync();
    void Add(TEntity entity);
    void Remove(TEntity entity);
    void Remove(int Id);
    IEnumerable<TEntity> Get();
    IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
    void Update(TEntity entity);
    void Dispose();
  }
}
