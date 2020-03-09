using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class IServiceBaseUrl_MockFactory
  {
    public static Mock<IServiceBaseUrl> Get(string ServersBaseServiceRootStu3, string ServersBaseServiceRootR4)
    {
      Mock<IServiceBaseUrl> IServiceBaseUrlMock = new Mock<IServiceBaseUrl>();
      IServiceBaseUrlMock.Setup(x =>
      x.Url(FhirVersion.Stu3)).Returns(new Uri(ServersBaseServiceRootStu3));
      IServiceBaseUrlMock.Setup(x =>
      x.Url(FhirVersion.R4)).Returns(new Uri(ServersBaseServiceRootR4));
      return IServiceBaseUrlMock;
    }
  }
}
