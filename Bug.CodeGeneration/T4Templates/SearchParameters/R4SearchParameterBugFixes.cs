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

    /// <summary>
    /// The SearchParameter named 'instantiates-canonical' for the RequestGroup resource is of search parameter type Reference and yet it has no target resources for the reference.
    /// This is intended to be a Reference(Any) type and should link all resources as it's target.
    /// This fix adds 3 resource types PlanDefinition, OperationDefinition and ActivityDefinition resources to the SearchParameter target property.
    /// It has not been confirmed that these are the correct resources but they appears to be set for other Search Parameters that have a property 
    /// called 'instantiates-canonical'. Better to have them than not I believe.
    /// </summary>
    /// <param name="Param"></param>
    /// <param name="ResourceNameKeyDic"></param>
    public static void FixFor_instantiatescanonical_SearchParameterFor_RequestGroup_NoTargetsListedForReferenceType(R4Model.SearchParameter Param)
    {
      string SearchParameterName = "instantiates-canonical";
      R4Model.ResourceType ResourceType = R4Model.ResourceType.RequestGroup;
      if (Param.Name == SearchParameterName && Param.Base.ToArray()[0] == ResourceType)
      {
        if (Param.Target != null || Param.Target.Count() == 0)
        {
          var ResourceTargetList = new List<R4Model.ResourceType?>();
          foreach (var ResourceName in R4Model.ModelInfo.SupportedResources)
          {                       
            ResourceTargetList.Add(R4Model.ResourceType.PlanDefinition);
            ResourceTargetList.Add(R4Model.ResourceType.OperationDefinition);
            ResourceTargetList.Add(R4Model.ResourceType.ActivityDefinition);
          }
          Param.Target = ResourceTargetList;
        }
        else
        {
          throw new Exception($"Review the bug fix for SearchParameter name {SearchParameterName} for resource {ResourceType.ToString()}, bug fix rule named FixForItemSearchParameterForLinkageResource_NoTargetsListedForReferenceType as it may have been fixed by the FHIR community.");
        }
      }
    }

  }
}
