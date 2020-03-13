using System;
using System.Linq;
using System.Collections.Generic;

namespace Bug.CodeGeneration.T4Templates.ResourceName
{
  public class ResourceNameGen
  {
    public Dictionary<string, int> Dic { get; set; }
    public ResourceNameGen()
    {
      Dic = new Dictionary<string, int>();
      int counter = 1;
      Dic.Add("Resource", counter);
      counter++;
      Dic.Add("DomainResource", counter);
      counter++;
      foreach (string ResourceName in Bug.CodeGeneration.Stu3.Stu3CodeGen.GetResourceNameList())
      {
        Dic.Add(ResourceName, counter);
        counter++;
      }
      foreach (string ResourceName in Bug.CodeGeneration.R4.R4CodeGen.GetResourceNameList())
      {
        if (!Dic.ContainsKey(ResourceName))
        {
          Dic.Add(ResourceName, counter);
          counter++;
        }
      }
      
    }
  }
}
