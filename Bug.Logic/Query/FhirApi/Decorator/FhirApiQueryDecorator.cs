using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi;

namespace Bug.Logic.Query.FhirApi.Decorator
{
  public class FhirApiQueryDecorator<TQuery, TResult> 
    : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult> 
  {
    private readonly IQueryHandler<TQuery, TResult> decorated;

    public FhirApiQueryDecorator(IQueryHandler<TQuery, TResult> decorated)
    {
      this.decorated = decorated;
    }

    public async Task<TResult> Handle(TQuery command)
    {

      return await this.decorated.Handle(command);

    }
  }
}
