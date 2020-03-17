extern alias R4;
extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using R4Model = R4.Hl7.Fhir.Model;
//using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Bug.CodeGeneration.T4Templates.ResourceName;
using System.Linq;
//using Bug.R4Fhir.Enums;

namespace Bug.CodeGeneration.T4Templates.SearchParameters
{
  public class SearchParameterGen
  {
    public List<SearchParameterModel> SearchParameterModelList { get; set; }
    public SearchParameterGen()
    {
      var ResourceNameKeyDictionary = new ResourceNameGen().Dic;
      DateTime Now = DateTime.Now;
      int SearchParameterIdCounter = 0;
      int TargetResourceCounter = 0;
      int ResourceCounter = 0;
      int ComponentModelCounter = 0;

      string FhirVersion = "Stu3";

      // ========== Stu3 =====================================================
      try
      {
        SearchParameterModelList = new List<SearchParameterModel>();
        Stu3Model.Bundle BundleStu3 = new Bug.CodeGeneration.Stu3.SearchParametersBundleLoader().Load();
        foreach (var Entry in BundleStu3.Entry)
        {
          if (Entry.Resource is Stu3Model.SearchParameter SearchParam)
          {
            if (!Stu3SearchParameterBugFixes.IsSearchParameterToBeSkiped(SearchParam))
            {
              //Bug fixes
              Stu3SearchParameterBugFixes.FixForItemSearchParameterForLinkageResource_NoTargetsListedForReferenceType(SearchParam);
              Stu3SearchParameterBugFixes.FixForSourceSearchParameterForLinkageResource_NoTargetsListedForReferenceType(SearchParam);

              var Param = new SearchParameterModel()
              {
                Id = SearchParameterIdCounter,
                Name = SearchParam.Name,
                Description = EscapeForSQL(SearchParam.Description.Value),
                FkSearchParamTypeId = (int)SearchParam.Type.Value,
                Url = SearchParam.Url,
                FhirPath = EscapeForSQL(SearchParam.Expression),
                FkFhirVersionId = 3,
                Created = GetSQLDateFormat(Now),
                Updated = GetSQLDateFormat(Now)
              };

              //Get the list of Resource this search parameter is for
              if (SearchParam.Base != null && SearchParam.Base.ToList().Count > 0)
              {
                Param.ResourceTypeList = new List<SearchParameterModel.SearchParameterResourceTypeModel>();
                foreach (var ResourceName in SearchParam.Base)
                {
                  Param.ResourceTypeList.Add(new SearchParameterModel.SearchParameterResourceTypeModel()
                  {
                    Id = ResourceCounter,
                    FkSearchParameterId = SearchParameterIdCounter,
                    FkResourceTypeId = ResourceNameKeyDictionary[ResourceName.ToString()],
                    Updated = GetSQLDateFormat(Now),
                    Created = GetSQLDateFormat(Now)
                  });
                  ResourceCounter++;
                }
              }
              else
              {
                throw new Exception($"An {FhirVersion} SearchParameter  has no Resource Base, so what resource is this SearchParameter for?. SearchParamter name is {SearchParam.Name} from {FhirVersion}.");
              }

              //Get the list of TargetResources this Reference search parameter is for
              if (SearchParam.Type.Value == Stu3Model.SearchParamType.Reference)
              {
                if (SearchParam.Target != null && SearchParam.Target.ToList().Count > 0)
                {
                  Param.TargetResourceNameList = new List<SearchParameterModel.SearchParameterTargetResourceTypeModel>();
                  
                  foreach (var ResourceName in SearchParam.Target)
                  {
                    Param.TargetResourceNameList.Add(new SearchParameterModel.SearchParameterTargetResourceTypeModel()
                    {
                      Id = TargetResourceCounter,
                      FkSearchParameterId = SearchParameterIdCounter,
                      FkResourceTypeId = ResourceNameKeyDictionary[ResourceName.ToString()],
                      Updated = GetSQLDateFormat(Now),
                      Created = GetSQLDateFormat(Now)
                    });
                    TargetResourceCounter++;
                  }
                }
                else
                {
                  throw new Exception($"An {FhirVersion} SearchParameter of type Reference has no Resource Targets . SearchParamter name is {SearchParam.Name} from {FhirVersion}.");
                }
              }

              //Get the list of Component for this Composite search parameter Type            
              if (SearchParam.Component != null && SearchParam.Component.ToList().Count > 0)
              {
                if (SearchParam.Type.Value != Stu3Model.SearchParamType.Composite)
                  throw new Exception($"An {FhirVersion} SearchParameter has Components however it is not a Composite searchParameter type, this should not happen. SearchParamter name is {SearchParam.Name} from {FhirVersion}.");
                Param.ComponentList = new List<SearchParameterModel.ComponentModel>();
                foreach (var Component in SearchParam.Component)
                {
                  Param.ComponentList.Add(new SearchParameterModel.ComponentModel()
                  {
                    Id = ComponentModelCounter,
                    FkSearchParameterId = SearchParameterIdCounter,
                    Definition = EscapeForSQL(Component.Definition.Reference),
                    Expression = EscapeForSQL(Component.Expression),
                    Updated = GetSQLDateFormat(Now),
                    Created = GetSQLDateFormat(Now)
                  });
                  ComponentModelCounter++;
                }
              }

              SearchParameterModelList.Add(Param);
            }
            SearchParameterIdCounter++;
          }
        }
      }
      catch (Exception Exec)
      {
        throw new Exception($"Error in setting the {typeof(SearchParameterModel).Name} from a {FhirVersion} SearchParameter resource in the definitions bundle for FHIR Stu3. See inner exception for more info ", Exec);
      }

      // ========== R4 =====================================================
      FhirVersion = "R4";
      try
      {
        R4Model.Bundle BundleR4 = new Bug.CodeGeneration.R4.SearchParametersBundleLoader().Load();
        foreach (var Entry in BundleR4.Entry)
        {
          if (Entry.Resource is R4Model.SearchParameter SearchParam)
          {
            if (!R4SearchParameterBugFixes.IsSearchParameterToBeSkiped(SearchParam))
            {
              var Param = new SearchParameterModel()
              {
                Id = SearchParameterIdCounter,
                Name = SearchParam.Name,
                Description = EscapeForSQL(SearchParam.Description.Value),
                FkSearchParamTypeId = (int)SearchParam.Type.Value,
                Url = SearchParam.Url,
                FhirPath = EscapeForSQL(SearchParam.Expression),
                FkFhirVersionId = 4,
                Updated = GetSQLDateFormat(Now),
                Created = GetSQLDateFormat(Now)
              };

              //Get the list of Resource this search parameter is for
              if (SearchParam.Base != null && SearchParam.Base.ToList().Count > 0)
              {
                Param.ResourceTypeList = new List<SearchParameterModel.SearchParameterResourceTypeModel>();
                foreach (var ResourceName in SearchParam.Base)
                {
                  Param.ResourceTypeList.Add(new SearchParameterModel.SearchParameterResourceTypeModel()
                  {
                    Id = ResourceCounter,
                    FkSearchParameterId = SearchParameterIdCounter,
                    FkResourceTypeId = ResourceNameKeyDictionary[ResourceName.ToString()],
                    Updated = GetSQLDateFormat(Now),
                    Created = GetSQLDateFormat(Now)
                  });
                  ResourceCounter++;
                }
              }
              else
              {
                throw new Exception($"An {FhirVersion} SearchParameter  has no Resource Base, so what resource is this SearchParameter for?. SearchParamter name is {SearchParam.Name} from {FhirVersion}.");
              }

              //Get the list of TargetResources this Reference search parameter is for
              if (SearchParam.Type.Value == R4Model.SearchParamType.Reference)
              {
                if (SearchParam.Target != null && SearchParam.Target.ToList().Count > 0)
                {
                  Param.TargetResourceNameList = new List<SearchParameterModel.SearchParameterTargetResourceTypeModel>();
                  foreach (var ResourceName in SearchParam.Target)
                  {
                    Param.TargetResourceNameList.Add(new SearchParameterModel.SearchParameterTargetResourceTypeModel()
                    {
                      Id = TargetResourceCounter,
                      FkResourceTypeId = ResourceNameKeyDictionary[ResourceName.ToString()],
                      FkSearchParameterId = SearchParameterIdCounter,
                      Updated = GetSQLDateFormat(Now),
                      Created = GetSQLDateFormat(Now)
                    });
                    TargetResourceCounter++;
                  }
                }
                else
                {
                  throw new Exception($"An {FhirVersion} SearchParameter of type Reference has no Resource Targets . SearchParamter name is {SearchParam.Name} from {FhirVersion}.");
                }
              }

              //Get the list of Component for this Composite search parameter Type
              if (SearchParam.Component != null && SearchParam.Component.ToList().Count > 0)
              {
                if (SearchParam.Type.Value != R4Model.SearchParamType.Composite)
                  throw new Exception($"An {FhirVersion} SearchParameter has Components however it is not a Composite searchParameter type, this should not happen. SearchParamter name is {SearchParam.Name} from {FhirVersion}.");
                Param.ComponentList = new List<SearchParameterModel.ComponentModel>();
                foreach (var Component in SearchParam.Component)
                {
                  Param.ComponentList.Add(new SearchParameterModel.ComponentModel()
                  {
                    Id = ComponentModelCounter,
                    FkSearchParameterId = SearchParameterIdCounter,
                    Definition = EscapeForSQL(Component.Definition),
                    Expression = EscapeForSQL(Component.Expression),
                    Updated = GetSQLDateFormat(Now),
                    Created = GetSQLDateFormat(Now)
                  });
                  ComponentModelCounter++;
                }               
              }
              SearchParameterModelList.Add(Param);
            }
            SearchParameterIdCounter++;
          }
        }
      }
      catch (Exception Exec)
      {
        throw new Exception($"Error in setting the {typeof(SearchParameterModel).Name} from a {FhirVersion} SearchParameter resource in the definitions bundle for FHIR R4. See inner exception for more info ", Exec);
      }
    }

    private string EscapeForSQL(string value)
    {
      if (value is null)
        return string.Empty;
      else
        return value.Replace(System.Environment.NewLine, " ").Replace("\r", "").Replace("\n", "").Replace("\"", "'").Replace("'", "''");
    }

    private string GetSQLDateFormat(DateTime value)
    {      
        return value.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }


  }

  public class SearchParameterModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int FkSearchParamTypeId { get; set; }
    public string Url { get; set; }
    public string FhirPath { get; set; }
    public int FkFhirVersionId { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public List<SearchParameterResourceTypeModel> ResourceTypeList { get; set; }
    public List<SearchParameterTargetResourceTypeModel> TargetResourceNameList { get; set; }
    public List<ComponentModel> ComponentList { get; set; }

    public class SearchParameterResourceTypeModel
    {
      public int Id { get; set; }
      public int FkSearchParameterId { get; set; }
      public int FkResourceTypeId { get; set; }
      public string  Updated { get; set; }
      public string Created { get; set; }

    }
    public class SearchParameterTargetResourceTypeModel
    {
      public int Id { get; set; }
      public int FkSearchParameterId { get; set; }
      public int FkResourceTypeId { get; set; }
      public string Updated { get; set; }
      public string Created { get; set; }
    }
    public class ComponentModel
    {
      public int Id { get; set; }
      public int FkSearchParameterId { get; set; }
      public string Definition { get; set; }
      public string Expression { get; set; }
      public string Updated { get; set; }
      public string Created { get; set; }
    }

  }

}
