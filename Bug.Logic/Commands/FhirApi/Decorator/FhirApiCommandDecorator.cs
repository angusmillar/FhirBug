using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Command;
using Bug.Logic.Command.FhirApi;

namespace Bug.Logic.Command.FhirApi.Decorator
{
  public class FhirApiCommandDecorator<TCommand, TOutcome> : ICommandHandler<TCommand, TOutcome>
    where TCommand : FhirApiCommand
    where TOutcome : FhirApiOutcome
  {
    private readonly ICommandHandler<TCommand, TOutcome> decorated;

    public FhirApiCommandDecorator(ICommandHandler<TCommand, TOutcome> decorated)
    {
      this.decorated = decorated;
    }

    public async Task<TOutcome> Handle(TCommand command)
    {

      return await this.decorated.Handle(command);

    }
  }
}
