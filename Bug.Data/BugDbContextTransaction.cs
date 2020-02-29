using Bug.Logic.Interfaces.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bug.Data
{
  public class BugDbContextTransaction : IBugDbContextTransaction, IDisposable
  {
    private readonly IDbContextTransaction IDbContextTransaction;
    public BugDbContextTransaction(IDbContextTransaction IDbContextTransaction)
    {
      this.IDbContextTransaction = IDbContextTransaction;
    }
    public Guid TransactionId => IDbContextTransaction.TransactionId;

    public void Commit()
    {
      IDbContextTransaction.Commit();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
      await IDbContextTransaction.CommitAsync(cancellationToken);
    }

    public void Dispose()
    {
      this.IDbContextTransaction.Dispose();
    }

    public void Rollback()
    {
      IDbContextTransaction.Rollback();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
      await IDbContextTransaction.RollbackAsync();
    }
  }
}
