using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api.CompositionRoot
{
  public class OperationOutComeSupportFactory: Bug.Logic.Interfaces.CompositionRoot.IOperationOutcomeSupportFactory
  {
    private readonly Container _container;
    public OperationOutComeSupportFactory(Container container)
    {
      this._container = container;
    }
    public Bug.Stu3Fhir.OperationOutCome.IStu3OperationOutComeSupport GetStu3()
    {
      return (Bug.Stu3Fhir.OperationOutCome.IStu3OperationOutComeSupport)_container.GetInstance(typeof(Bug.Stu3Fhir.OperationOutCome.IStu3OperationOutComeSupport));
    }
    public Bug.R4Fhir.OperationOutCome.IR4OperationOutComeSupport GetR4()
    {
      return (Bug.R4Fhir.OperationOutCome.IR4OperationOutComeSupport)_container.GetInstance(typeof(Bug.R4Fhir.OperationOutCome.IR4OperationOutComeSupport));
    }
  }
}
