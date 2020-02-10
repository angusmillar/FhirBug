using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Command;
using Bug.Common.Enums;
using Bug.Logic.Command.FhirApi;
using Microsoft.Extensions.Logging;

namespace Bug.Logic.Command.FhirApi.Decorator
{
  public class FhirApiCommandLoggingDecorator<TCommand, TOutcome> : ICommandHandler<TCommand, TOutcome>
    where TCommand : FhirApiCommand
    where TOutcome : FhirApiOutcome
  {
    private readonly ICommandHandler<TCommand, TOutcome> Decorated;
    private readonly ILogger ILogger;


    public FhirApiCommandLoggingDecorator(ICommandHandler<TCommand, TOutcome> decorated, ILogger logger)
    {
      this.Decorated = decorated;
      this.ILogger = logger;
    }

    public async Task<TOutcome> Handle(TCommand command)
    {
      ILogger.LogInformation($"{new String('-', 80)}");
      ILogger.LogInformation($"Request Uri: {command.RequestUri.ToString()}");
      ILogger.LogInformation($"Major Fhir version: {command.FhirMajorVersion.GetLiteral()}");      
      //if (command is Bug.Logic.Commands.FhirApi.Update.UpdateCommand updateCommand)
      //{
      //  updateCommand.resource;
      //}
      return await this.Decorated.Handle(command);
    }
  }
}
