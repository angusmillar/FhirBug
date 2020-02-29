using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IBugDbContextTransaction: IDisposable
  {
    Guid TransactionId { get; }

    //
    // Summary:
    //     Commits all changes made to the database in the current transaction.
    void Commit();
    //
    // Summary:
    //     Commits all changes made to the database in the current transaction asynchronously.
    //
    // Parameters:
    //   cancellationToken:
    //     The cancellation token.
    //
    // Returns:
    //     A System.Threading.Tasks.Task representing the asynchronous operation.
    Task CommitAsync(CancellationToken cancellationToken = default);
    //
    // Summary:
    //     Discards all changes made to the database in the current transaction.
    void Rollback();
    //
    // Summary:
    //     Discards all changes made to the database in the current transaction asynchronously.
    //
    // Parameters:
    //   cancellationToken:
    //     The cancellation token.
    //
    // Returns:
    //     A System.Threading.Tasks.Task representing the asynchronous operation.
    Task RollbackAsync(CancellationToken cancellationToken = default);
  }
}
