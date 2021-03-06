﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Query;
using Bug.Common.Enums;
using Bug.Logic.Query.FhirApi;
using Microsoft.Extensions.Logging;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Interfaces.Transaction;

namespace Bug.Logic.Query.FhirApi.Decorator
{
  public class FhirApiQueryDbTransactionDecorator<TQuery, TResult>
    : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
  {
    private readonly IQueryHandler<TQuery, TResult> decorated;
    private readonly IUnitOfWork IUnitOfWork;


    public FhirApiQueryDbTransactionDecorator(IQueryHandler<TQuery, TResult> decorated, IUnitOfWork IUnitOfWork)
    {
      this.decorated = decorated;
      this.IUnitOfWork = IUnitOfWork;
    }

    public async Task<TResult> Handle(TQuery query)
    {
      if (query is null)
        throw new ArgumentNullException(paramName: nameof(query));

      using IBugDbContextTransaction Transaction = await IUnitOfWork.BeginTransactionAsync();

      try
      {
        TResult Result = await this.decorated.Handle(query);
        if (Result is ITransactional TransactionalResult)
        {
          if (TransactionalResult.CommitTransaction)
          {
            await Transaction.CommitAsync();
          }
          else
          {
            Transaction.Rollback();
          }
          return Result;
        }
        else
        {
          Transaction.Rollback();
          throw new System.ApplicationException($"Internal Server Error: Detected a transactional query attributed class who's result did not implement the ITransactional interface. The Query was of type {query.GetType().FullName}. The transaction was rolled back.");
        }
      }
      catch (Exception Exec)
      {
        Transaction.Rollback();
        throw Exec;
      }
    }
  }
}
