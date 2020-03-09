using Bug.Common.FhirTools;
using Bug.Logic.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class IFhirResourceNameSupport_MockFactory
  {
    public static Mock<IFhirResourceNameSupport> Get(string resourceName)
    {
      Mock<IFhirResourceNameSupport> IFhirResourceNameSupportMock = new Mock<IFhirResourceNameSupport>();
      IFhirResourceNameSupportMock.Setup(x => x.GetName(It.IsAny<FhirResource>())).Returns(resourceName);      
      return IFhirResourceNameSupportMock;
    }
  }
}
