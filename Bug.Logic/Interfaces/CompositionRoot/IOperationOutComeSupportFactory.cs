﻿
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IOperationOutcomeSupportFactory
  {
    R4Fhir.OperationOutCome.IR4OperationOutComeSupport GetR4();
    Stu3Fhir.OperationOutCome.IStu3OperationOutComeSupport GetStu3();
  }
}