
extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.CodeGeneration.Stu3
{
  public static class Stu3CodeGen
  {
    public static List<string> GetResourceNameList()
    {
      return Stu3Model.ModelInfo.SupportedResources;
    }
    public static Stu3Model.Bundle GetSearchParameterBundle()
    {
      return new SearchParametersBundleLoader().Load();
    }
  }
}
