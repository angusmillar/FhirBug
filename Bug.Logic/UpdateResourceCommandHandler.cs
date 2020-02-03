using Bug.Logic.Commands;
using Bug.Common.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic
{
  public class UpdateResourceCommandHandler : ICommandHandler<UpdateResourceCommand>
  {
    private readonly ILogger ILogger;

    public UpdateResourceCommandHandler(ILogger logger)
    {
      this.ILogger = logger;
    }

    public async Task Handle(UpdateResourceCommand command)
    {
      ILogger.LogInformation($"FhirVersion: {command.FhirMajorVersion.GetLiteral()}");
      ILogger.LogInformation($"FhirVersion: {command.RequestUri.ToString()}");
      
      await Task.CompletedTask;
    }
  }
}
