using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Commands
{
  public interface ICommandHandler<TCommand>
  {
    Task Handle(TCommand command);
  }
}
