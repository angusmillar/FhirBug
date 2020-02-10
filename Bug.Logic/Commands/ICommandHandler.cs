using Bug.Logic.Command.FhirApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Command
{
  public interface ICommandHandler<TCommand, TOutcome>    
    where TCommand : FhirApiCommand
    where TOutcome : FhirApiOutcome
  {
    Task<TOutcome> Handle(TCommand command);
  }
}
