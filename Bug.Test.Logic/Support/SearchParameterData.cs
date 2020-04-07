using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.Support
{
  public static class SearchParameterData
  {
    public static List<SearchParameter> GetSearchParameterList(Bug.Common.Enums.FhirVersion fhirVersion)
    {
      return new List<SearchParameter>()
        {
          new SearchParameter()
          {
             Id = 1,
             Name = "code",
             Description = "Bla bla bla",
             FhirPath = "AllergyIntolerance.code | AllergyIntolerance.reaction.substance | Condition.code | (DeviceRequest.code as CodeableConcept) | DiagnosticReport.code | FamilyMemberHistory.condition.code | List.code | Medication.code | (MedicationAdministration.medication as CodeableConcept) | (MedicationDispense.medication as CodeableConcept) | (MedicationRequest.medication as CodeableConcept) | (MedicationStatement.medication as CodeableConcept) | Observation.code | Procedure.code | ServiceRequest.code",
             Url = "http://hl7.org/fhir/SearchParameter/clinical-code",
             SearchParamTypeId = Common.Enums.SearchParamType.Token,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.DiagnosticReport},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Medication}
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "Expression", Definition= "Url Definition" } 
             }
          },
          new SearchParameter()
          {
             Id = 2,
             Name = "value-concept",
             Description = "Bla bla bla",
             FhirPath = "(Observation.value as CodeableConcept)",
             Url = "http://hl7.org/fhir/SearchParameter/Observation-value-concept",
             SearchParamTypeId = Common.Enums.SearchParamType.Token,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "Expression", Definition= "Url Definition" } 
             }
          },
          new SearchParameter()
          {
             Id = 3,
             Name = "code-value-concept",
             Description = "Bla bla bla",
             FhirPath = "Observation",
             Url = "http://hl7.org/fhir/SearchParameter/Observation-code-value-concept",
             SearchParamTypeId = Common.Enums.SearchParamType.Composite,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },
               new SearchParameterComponent() {  Id = 1,  Expression = "value.as(CodeableConcept)", Definition= "http://hl7.org/fhir/SearchParameter/Observation-value-concept" }
             }
          },
          new SearchParameter()
          {
             Id = 4,
             Name = "family",
             Description = "Bla bla bla",
             FhirPath = "Patient.name.family | Practitioner.name.family",
             Url = "http://hl7.org/fhir/SearchParameter/individual-family",
             SearchParamTypeId = Common.Enums.SearchParamType.String,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Patient},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Practitioner},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             }
          },
          new SearchParameter()
          {
             Id = 5,
             Name = "subject",
             Description = "Bla bla bla",
             FhirPath = "Observation.subject",
             Url = "http://hl7.org/fhir/SearchParameter/Observation-subject",
             SearchParamTypeId = Common.Enums.SearchParamType.Reference,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Group},
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Device},
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Patient},
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Location},
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             }
          },
          new SearchParameter()
          {
             Id = 6,
             Name = "encounter",
             Description = "Bla bla bla",
             FhirPath = "Composition.encounter | DeviceRequest.encounter | DiagnosticReport.encounter | DocumentReference.context.encounter | Flag.encounter | List.encounter | NutritionOrder.encounter | Observation.encounter | Procedure.encounter | RiskAssessment.encounter | ServiceRequest.encounter | VisionPrescription.encounter",
             Url = "http://hl7.org/fhir/SearchParameter/clinical-encounter",
             SearchParamTypeId = Common.Enums.SearchParamType.Reference,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Procedure},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Encounter}
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             }
          },
          new SearchParameter()
          {
             Id = 7,
             Name = "value-quantity",
             Description = "Bla bla bla",
             FhirPath = "(Observation.value as Quantity) | (Observation.value as SampledData)",
             Url = "http://hl7.org/fhir/SearchParameter/Observation-value-quantity",
             SearchParamTypeId = Common.Enums.SearchParamType.Quantity,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               //new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Encounter}
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             }
          },
          new SearchParameter()
          {
             Id = 8,
             Name = "date",
             Description = "Bla bla bla",
             FhirPath = "AllergyIntolerance.recordedDate | CarePlan.period | CareTeam.period | ClinicalImpression.date | Composition.date | Consent.dateTime | DiagnosticReport.effective | Encounter.period | EpisodeOfCare.period | FamilyMemberHistory.date | Flag.period | Immunization.occurrence | List.date | Observation.effective | Procedure.performed | (RiskAssessment.occurrence as dateTime) | SupplyRequest.authoredOn",
             Url = "http://hl7.org/fhir/SearchParameter/clinical-date",
             SearchParamTypeId = Common.Enums.SearchParamType.Date,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Procedure},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.CarePlan},
               //..and others
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               //new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Encounter}
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             }
          },
          new SearchParameter()
          {
             Id = 9,
             Name = "probability",
             Description = "Bla bla bla",
             FhirPath = "RiskAssessment.prediction.probability",
             Url = "http://hl7.org/fhir/SearchParameter/RiskAssessment-probability",
             SearchParamTypeId = Common.Enums.SearchParamType.Number,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.RiskAssessment},
               //..and others
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               //new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Encounter}
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             }
          },
          new SearchParameter()
          {
             Id = 11,
             Name = "url",
             Description = "Bla bla bla",
             FhirPath = "CapabilityStatement.url | CodeSystem.url | CompartmentDefinition.url | ConceptMap.url | GraphDefinition.url | ImplementationGuide.url | MessageDefinition.url | OperationDefinition.url | SearchParameter.url | StructureDefinition.url | StructureMap.url | TerminologyCapabilities.url | ValueSet.url",
             Url = "http://hl7.org/fhir/SearchParameter/conformance-url",
             SearchParamTypeId = Common.Enums.SearchParamType.Uri,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.CodeSystem},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.ValueSet},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.StructureDefinition},
               //..and others
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               //new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Encounter}
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             }
          }
        };
    }
  }
}
