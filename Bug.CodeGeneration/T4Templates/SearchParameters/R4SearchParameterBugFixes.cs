extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.CodeGeneration.T4Templates.SearchParameters
{
  public static class R4SearchParameterBugFixes
  {
    /// <summary>
    /// Filters out SearchParameters that can not be easily fixed
    /// </summary>
    /// <param name="Param"></param>
    /// <returns></returns>
    public static bool IsSearchParameterToBeSkiped(R4Model.SearchParameter Param)
    {
            
      if (Param.Name == "instantiates-canonical" && Param.Base.ToArray()[0] ==  R4Model.ResourceType.RequestGroup)
      {
        if (Param.Target != null && Param.Target.Count() > 0)
          throw new Exception($"Review the skipping of the SearchParameter name instantiates-canonical for resource RequestGroup, skip rule named IsSearchParameterToBeSkiped as it may have been fixed by the FHIR community.");
        return true;
      }
      else
      {
        return false;
      }
    }

    
  }
}
