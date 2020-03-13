extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.CodeGeneration.T4Templates.SearchParameters
{
  public static class Stu3SearchParameterBugFixes
  {

    /// <summary>
    /// Filters out SearchParameters that can not be easily fixed
    /// </summary>
    /// <param name="Param"></param>
    /// <returns></returns>
    public static bool IsSearchParameterToBeSkiped(Stu3Model.SearchParameter Param)
    {
      if (Param.Name == "<#SearchParameterName#>" && Param.Base.ToArray()[0] == Stu3Model.ResourceType.Resource)
      {        
        return true;
      }
      else
      {
        return false;
      }
    }


    /// <summary>
    /// The SearchParameter named 'item' for the Linkage resource is of search parameter type Reference and yet it has no target resources for the reference.
    /// This is intended to be a Reference(Any) type and should link all resources as it's target.
    /// This fix adds all resources to the SearchParameter.Target property
    /// </summary>
    /// <param name="Param"></param>
    /// <param name="ResourceNameKeyDic"></param>
    public static void FixForItemSearchParameterForLinkageResource_NoTargetsListedForReferenceType(Stu3Model.SearchParameter Param)
    {
      string SearchParameterName = "item";
      Stu3Model.ResourceType ResourceType = Stu3Model.ResourceType.Linkage;
      if (Param.Name == SearchParameterName && Param.Base.ToArray()[0] == ResourceType)
      {
        if (Param.Target != null || Param.Target.Count() == 0)
        {
          var ResourceTargetList = new List<Stu3Model.ResourceType?>();
          foreach (var ResourceName in Stu3Model.ModelInfo.SupportedResources)
          {
            Stu3Model.ResourceType? Type = Stu3Model.ModelInfo.FhirTypeNameToResourceType(ResourceName);
            if (Type.HasValue)
            {
              ResourceTargetList.Add(Type.Value);
            }
          }
          Param.Target = ResourceTargetList;
        }
        else
        {
          throw new Exception($"Review the bug fix for SearchParameter name {SearchParameterName} for resource {ResourceType.ToString()}, bug fix rule named FixForItemSearchParameterForLinkageResource_NoTargetsListedForReferenceType as it may have been fixed by the FHIR community.");
        }
      }
    }

    /// <summary>
    /// The SearchParameter named 'source' for the Linkage resource is of search parameter type Reference and yet it has no target resources for the reference.
    /// This is intended to be a Reference(Any) type and should link all resources as it's target.
    /// This fix adds all resources to the SearchParameter.Target property
    /// This fix does not address a deeper problem that the description of this parameter says "Matches on any item in the Linkage with a type of 'source'". 
    /// So this should be a composite SearchParameter to achieve this! 
    /// </summary>
    /// <param name="Param"></param>
    /// <param name="ResourceNameKeyDic"></param>
    public static void FixForSourceSearchParameterForLinkageResource_NoTargetsListedForReferenceType(Stu3Model.SearchParameter Param)
    {
      string SearchParameterName = "source";
      Stu3Model.ResourceType ResourceType = Stu3Model.ResourceType.Linkage;
      if (Param.Name == SearchParameterName && Param.Base.ToArray()[0] == ResourceType)
      {
        if (Param.Target != null || Param.Target.Count() == 0)
        {
          var ResourceTargetList = new List<Stu3Model.ResourceType?>();
          foreach (var ResourceName in Stu3Model.ModelInfo.SupportedResources)
          {
            Stu3Model.ResourceType? Type = Stu3Model.ModelInfo.FhirTypeNameToResourceType(ResourceName);
            if (Type.HasValue)
            {
              ResourceTargetList.Add(Type.Value);
            }
          }
          Param.Target = ResourceTargetList;
        }
        else
        {
          throw new Exception($"Review the bug fix for SearchParameter name {SearchParameterName} for resource {ResourceType.ToString()}, bug fix rule named FixForSourceSearchParameterForLinkageResource_NoTargetsListedForReferenceType as it may have been fixed by the FHIR community.");
        }
      }
    }
  }
}
