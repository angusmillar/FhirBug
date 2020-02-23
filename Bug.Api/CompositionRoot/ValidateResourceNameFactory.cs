using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api.CompositionRoot
{
  public class ValidateResourceNameFactory : Bug.Logic.Interfaces.CompositionRoot.IValidateResourceNameFactory
  {
    private readonly Container _container;
    public ValidateResourceNameFactory(Container container)
    {
      this._container = container;
    }
    public Bug.Stu3Fhir.ResourceSupport.IStu3ValidateResourceName GetStu3()
    {
      return (Bug.Stu3Fhir.ResourceSupport.IStu3ValidateResourceName)_container.GetInstance(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3ValidateResourceName));
    }
    public Bug.R4Fhir.ResourceSupport.IR4ValidateResourceName GetR4()
    {
      return (Bug.R4Fhir.ResourceSupport.IR4ValidateResourceName)_container.GetInstance(typeof(Bug.R4Fhir.ResourceSupport.IR4ValidateResourceName));
    }
  }
}
