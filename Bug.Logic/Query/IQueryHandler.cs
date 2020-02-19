using Bug.Logic.Query.FhirApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Query
{
  public interface IQueryHandler<TQuery, TResult>    
    where TQuery : IQuery<TResult>    
  {
    Task<TResult> Handle(TQuery query);
  }
}
