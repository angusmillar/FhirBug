using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Command;
using Bug.Common.Enums;
using Bug.Logic.Command.FhirApi;
using Microsoft.Extensions.Logging;
using Bug.Logic.Command.FhirApi.Update;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.DomainModel;
using Bug.Common.Exceptions;

namespace Bug.Logic.Command.FhirApi.Create.Decorator
{
  public class CreateValidatorDecorator<TCommand, TOutcome> : ICommandHandler<TCommand, TOutcome>
    where TCommand : CreateCommand
    where TOutcome : FhirApiOutcome
  {
    private readonly ICommandHandler<TCommand, TOutcome> Decorated;    
    private readonly IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory;        
    private readonly IOperationOutComeSupportFactory IOperationOutComeSupportFactory;


    public CreateValidatorDecorator(ICommandHandler<TCommand, TOutcome> decorated,
      IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory,      
      IOperationOutComeSupportFactory IOperationOutComeSupportFactory)
    {
      this.Decorated = decorated;      
      this.IFhirResourceIdSupportFactory = IFhirResourceIdSupportFactory;      
      this.IOperationOutComeSupportFactory = IOperationOutComeSupportFactory;      
    }

    public async Task<TOutcome> Handle(TCommand command)
    {
      
      IsFhirMajorVersionValid(command.FhirMajorVersion);
      if (!ValidateResourceIdIsEmpty(command.FhirMajorVersion, command.Resource, out object OperationOutComeResource))
      {
        if (OperationOutComeResource != null)
        {
          var FhirApiOutcome = new FhirApiOutcome()
          {
            FhirMajorVersion = command.FhirMajorVersion,
            HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
            Resource = OperationOutComeResource
          };

          return FhirApiOutcome as TOutcome;
        }
      }

      return await this.Decorated.Handle(command);

    }

    private bool ValidateResourceIdIsEmpty(FhirMajorVersion fhirMajorVersion, object resource, out object OperationOutComeResource)
    {
      OperationOutComeResource = null;
      string ErrorMessage = $"The create (POST) interaction creates a new resource in a server-assigned location. If the client wishes to have control over the id of a newly submitted resource, it should use the update interaction instead. If you intend to allow the server to perform a create and assign an id then please remove the id found in the resource submitted. The Resource submitted was found to contain the id:";
      if (fhirMajorVersion == FhirMajorVersion.Stu3)
      {
        var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();
        if (!string.IsNullOrWhiteSpace(FhirResourceIdSupport.GetFhirId(resource)))
        {
          OperationOutComeResource = IOperationOutComeSupportFactory.GetStu3().GetError(new string[] { ErrorMessage + FhirResourceIdSupport.GetFhirId(resource) });
          return false;
        }
      }
      else if (fhirMajorVersion == FhirMajorVersion.R4)
      {
        var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetR4();
        if (!string.IsNullOrWhiteSpace(FhirResourceIdSupport.GetFhirId(resource)))
        {
          OperationOutComeResource = IOperationOutComeSupportFactory.GetR4().GetError(new string[] { ErrorMessage + FhirResourceIdSupport.GetFhirId(resource) });
          return false;
        }
      }
      return true;
    }

    private void IsFhirMajorVersionValid(FhirMajorVersion fhirMajorVersion)
    {
      FhirMajorVersion[] fhirMajorVersionsAllowed = new FhirMajorVersion[] { FhirMajorVersion.Stu3, FhirMajorVersion.R4 };
      if (!fhirMajorVersionsAllowed.Contains(fhirMajorVersion))
      {
        throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }
  }
}
