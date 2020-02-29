using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IUnitOfWork
  {
    Task<IBugDbContextTransaction> BeginTransactionAsync();
  }
}
