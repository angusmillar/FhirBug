extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;
using R4Serialization = R4.Hl7.Fhir.Serialization;
using System;
using Bug.CodeGeneration.Zip;
using Newtonsoft.Json;

namespace Bug.CodeGeneration.R4
{
  public class SearchParametersBundleLoader
  {
    public R4Model.Bundle Load()
    {
      string ZipFileName = "search-parameters.json";
      ZipFileJsonLoader ZipFileJsonLoader = new ZipFileJsonLoader();
      JsonReader JsonReader = ZipFileJsonLoader.Load(ProjectResource.definitions_4_0_1_json, ZipFileName);    
      try
      {        
        R4Model.Resource Resource = ParseJson(JsonReader);
        if (Resource is R4Model.Bundle Bundle)
        {
          return Bundle;
        }
        else
        {
          throw new Exception($"Exception thrown when casting the json resource to a Bundle from the R4 FHIR specification {ZipFileName} file.");
        }
      }
      catch (Exception Exec)
      {
        throw new Exception($"Exception thrown when de-serializing FHIR resource bundle from the R4 FHIR specification {ZipFileName} file. See inner exception for more info.", Exec);
      }
    }

    private R4Model.Resource ParseJson(JsonReader reader)
    {      
      try
      {
        var FhirJsonParser = new R4Serialization.FhirJsonParser();
        return FhirJsonParser.Parse<R4Model.Resource>(reader);
      }
      catch (Exception oExec)
      {
        throw new Exception("Error parsing Stu3 json to FHIR Resource, See inner exception info more info", oExec);
      }
    }
  }
}

