extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.CodeGeneration.R4
{
  public static class R4CodeGen
  {
    public static List<string> GetResourceNameList()
    {
      return R4Model.ModelInfo.SupportedResources;
    }
  }
}
