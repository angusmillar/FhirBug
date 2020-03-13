extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Stu3Serialization = Stu3.Hl7.Fhir.Serialization;
using System;
using Bug.CodeGeneration.Zip;
using Newtonsoft.Json;

namespace Bug.CodeGeneration.Stu3
{
  public class SearchParametersBundleLoader
  {
    public Stu3Model.Bundle Load()
    {
      string ZipFileName = "search-parameters.json";
      ZipFileJsonLoader ZipFileJsonLoader = new ZipFileJsonLoader();
      JsonReader JsonReader = ZipFileJsonLoader.Load(Bug.CodeGeneration.ProjectResource.definitions_3_0_2_json, ZipFileName);    
      try
      {        
        Stu3Model.Resource Resource = ParseJson(JsonReader);
        if (Resource is Stu3Model.Bundle Bundle)
        {
          return Bundle;
        }
        else
        {
          throw new Exception($"Exception thrown when casting the json resource to a Bundle from the Stu3 FHIR specification {ZipFileName} file.");
        }
      }
      catch (Exception Exec)
      {
        throw new Exception($"Exception thrown when de-serializing FHIR resource bundle from the Stu3 FHIR specification {ZipFileName} file. See inner exception for more info.", Exec);
      }
    }

    private Stu3Model.Resource ParseJson(JsonReader reader)
    {
      try
      {
        var FhirJsonParser = new Stu3Serialization.FhirJsonParser();
        return FhirJsonParser.Parse<Stu3Model.Resource>(reader);
      }
      catch (Exception oExec)
      {
        throw new Exception("Error parsing R4 Json to FHIR Resource, See inner exception info more info", oExec);
      }
    }
  }
}

